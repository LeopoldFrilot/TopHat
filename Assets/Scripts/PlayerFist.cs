using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public enum PlayerFistState
{
    Idle,
    Windup,
    Launch,
    Retract,
    Block
}
public enum PlayerFistSide
{
    In,
    Out
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerFist : MonoBehaviour
{
    [SerializeField] private float maxWindup = 10f;
    [SerializeField] private float windupSpeed = 10f;
    [SerializeField] private float launchStrength = 5f;
    [SerializeField] private float retractTime = .5f;
    [SerializeField] private float baseWindup = 1f;
    [SerializeField] private float idleFollowSpeed = 5f;
    [SerializeField] private float parryTime = .5f;
    [SerializeField] private Vector2 windupOffset = new(1f, 0);
    [SerializeField] private Transform artTransform;
    [SerializeField] private List<Collider2D> fistColliders = new();
    
    [Header("Audio")]
    [SerializeField] private AudioClip fistHitSound;
    [SerializeField] private AudioClip fistBlockedSound;
    [SerializeField] private AudioClip windupSound;
    [SerializeField] private AudioClip fistReleaseSound;

    private Vector2 currentOffset;
    private float windup = 0;
    private PlayerFistState currentState;
    
    private Rigidbody2D rb2d;
    private Transform restingPosition;
    private Transform blockPosition;
    private Fighter _fighter;
    private Tweener idleFollow = null;
    private Tweener windupShake = null;
    private Tweener retract = null;
    private Tweener blockGrowth = null;
    private Tweener blockRotate = null;
    private TweenCallback shakeTweenComplete;
    private AudioSource windupAudio;
    private Coroutine blockRoutine = null;
    private Coroutine parryRoutine = null;
    private bool consumed = false;
    public PlayerFistSide fistID;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        rb2d.gravityScale = 0f;
        rb2d.constraints = RigidbodyConstraints2D.FreezePositionY;
    }

    private void Start()
    {
        windupAudio = GameWizard.Instance.GetComponent<AudioHub>().SetUpLoopingAudio(windupSound, .1f);
        SwitchState(PlayerFistState.Idle, true);
        shakeTweenComplete = () =>
        {
            windupShake = artTransform.DOShakePosition(.1f, .003f * (baseWindup + windup) * (baseWindup + windup)).SetRelative()
                .OnComplete(shakeTweenComplete);
        };
    }

    private void Update()
    {
        if (currentState == PlayerFistState.Windup)
        {
            windup = Mathf.Clamp(windup + Time.deltaTime * windupSpeed, 0, maxWindup - baseWindup);
            currentOffset = (windup + baseWindup) * windupOffset;
            transform.position = restingPosition.position - new Vector3((_fighter.IsFacingLeft() ? -1 : 1) * currentOffset.x, currentOffset.y, 0);
        }
        else if(currentState == PlayerFistState.Launch)
        {
            if (Mathf.Abs(rb2d.linearVelocityX) <= .4f)
            {
                Retract();
            }
        }
    }

    public void Iniialize(Fighter fighter, Transform restingPosition, Transform blockPosition, PlayerFistSide fistID)
    {
        this.restingPosition = restingPosition;
        this._fighter = fighter;
        this.blockPosition = blockPosition;
        this.fistID = fistID;
    }

    public PlayerFistState GetCurrentState()
    {
        return currentState;
    }

    private void SwitchState(PlayerFistState newState, bool force = false)
    {
        if (!force && (newState == currentState))
        {
            return;
        }
        
        PlayerFistState prevState = currentState;
        switch (prevState)
        {
            case PlayerFistState.Idle:
                idleFollow.Complete();
                break;
            
            case PlayerFistState.Windup:
                windupShake.Complete();
                windupShake.Kill();
                windupAudio.Stop();
                break;
        }
        
        currentState = newState;
        switch (currentState)
        {
            case PlayerFistState.Idle:
                idleFollow = transform.DOMove(restingPosition.position, idleFollowSpeed).SetSpeedBased(true).SetEase(Ease.OutCubic);
                idleFollow.OnUpdate(() => idleFollow.ChangeEndValue(restingPosition.position, true).Restart());
                windup = 0;
                break;
                
            case PlayerFistState.Windup:
                consumed = false;
                shakeTweenComplete.Invoke();
                windupAudio.Play();
                break;
            
            case PlayerFistState.Launch:
                rb2d.bodyType = RigidbodyType2D.Dynamic;
                rb2d.AddForceX((_fighter.IsFacingLeft() ? -1 : 1) * (windup + baseWindup) * launchStrength, ForceMode2D.Impulse);
                EventHub.TriggerPlaySoundRequested(fistReleaseSound, .5f);
                break;
            
            case PlayerFistState.Retract:
                rb2d.bodyType = RigidbodyType2D.Kinematic;
                retract = transform.DOMove(restingPosition.position, retractTime).SetEase(Ease.InCubic)
                    .OnComplete(()=>SwitchState(PlayerFistState.Idle));
                break;
        }

        foreach (var fistCollider in fistColliders)
        {
            fistCollider.enabled = currentState is PlayerFistState.Block or PlayerFistState.Launch;
        }
    }


    public void StartWindup()
    {
        SwitchState(PlayerFistState.Windup);
    }

    public void Retract()
    {
        SwitchState(PlayerFistState.Retract);
    }

    public void Launch()
    {
        SwitchState(PlayerFistState.Launch);
    }

    public void StartBlock()
    {
        SwitchState(PlayerFistState.Block);
        if (blockRoutine != null)
        {
            StopCoroutine(blockRoutine);
        }

        blockRoutine = StartCoroutine(BlockIdle());
        blockGrowth = artTransform.DOScale(Vector3.one * 1.5f, .2f).SetEase(Ease.InCubic);
        blockRotate = artTransform.DORotate(new Vector3(0, 0, _fighter.IsFacingLeft() ? -90 : 90), .2f, RotateMode.Fast);
    }

    private IEnumerator BlockIdle()
    {
        while (true)
        {
            if (parryRoutine == null)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    blockPosition.position,
                    Time.deltaTime * 5f
                );
            }
            
            yield return null;
        }
    }

    private IEnumerator ParryAnimation(Vector3 TargetPosition)
    {
        float time = 0;
        while (time < parryTime)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                TargetPosition,
                Time.deltaTime * 15f);
            time += Time.deltaTime;
            yield return null;
        }
        
        parryRoutine = null;

        if (blockRoutine == null)
        {
            transform.position = restingPosition.position;
        }
        
    }

    public void StopBlock(bool fast)
    {
        blockGrowth.Complete();
        blockGrowth = artTransform.DOScale(Vector3.one, .2f).SetEase(Ease.InCubic);
        blockRotate = artTransform.DORotate(new Vector3(0, 0, 0), .2f, RotateMode.Fast);
        transform.position = restingPosition.position;
        if (blockRoutine != null)
        {
            StopCoroutine(blockRoutine);
            blockRoutine = null;
        }
        
        blockGrowth.Complete();
        SwitchState(PlayerFistState.Idle);
    }

    public void HandleCollisionWithPlayer(Fighter player1, bool wasBlocked)
    {
        consumed = true;
        rb2d.linearVelocity = Vector2.zero;
        artTransform.DOShakeScale(.5f);
        EventHub.TriggerPlaySoundRequested(wasBlocked ? fistBlockedSound : fistHitSound);
    }

    public Fighter GetOwner()
    {
        return _fighter;
    }

    public void PauseFistControl()
    {
        transform.position = restingPosition.position;
        if (blockRoutine != null)
        {
            StopCoroutine(blockRoutine);
            blockRoutine = null;
        }
        idleFollow.Complete();
    }

    public void ResumeFistControl()
    {
        if (currentState == PlayerFistState.Idle)
        {
            idleFollow = transform.DOMove(restingPosition.position, idleFollowSpeed).SetSpeedBased(true).SetEase(Ease.OutCubic);
            idleFollow.OnUpdate(() => idleFollow.ChangeEndValue(restingPosition.position, true).Restart());
        }
    }

    public float GetWindupNormalized()
    {
        return (windup+baseWindup) / maxWindup;
    }

    public bool IsConsumed()
    {
        return consumed;
    }

    public void Reset()
    {
        StopBlock(true);
        SwitchState(PlayerFistState.Idle);
    }

    public void ForceParryAtPosition(Vector3 transformPosition)
    {
        if (parryRoutine != null)
        {
            StopCoroutine(parryRoutine);
        }
        
        parryRoutine = StartCoroutine(ParryAnimation(transformPosition));
    }
}