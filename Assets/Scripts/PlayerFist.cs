using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

public enum PlayerFistState
{
    Idle,
    Windup,
    Launch,
    Retract,
    Block,
    Grapple
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerFist : MonoBehaviour
{
    [SerializeField] private Transform artTransform;
    [SerializeField] private List<Collider2D> fistColliders = new List<Collider2D>();
    [SerializeField] private SpriteRenderer highlightRenderer;
    
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
    private Coroutine blockRoutine = null;
    private Tweener blockGrowth = null;
    private Tweener blockRotate = null;
    private TweenCallback shakeTweenComplete;
    private StudioEventEmitter windupAudio;
    private bool consumed = false;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        rb2d.gravityScale = 0f;
        rb2d.constraints = RigidbodyConstraints2D.FreezePositionY;
        highlightRenderer.enabled = false;
    }

    private void Start()
    {
        windupAudio = GameWizard.Instance.audioHub.SetupLoopingClip(Help.Audio.punchChargeUp);
        SwitchState(PlayerFistState.Idle, true);
        shakeTweenComplete = () =>
        {
            windupShake = artTransform.DOShakePosition(.1f, .003f * (Help.Tunables.baseWindup + windup) * (Help.Tunables.baseWindup + windup)).SetRelative()
                .OnComplete(shakeTweenComplete);
        };
    }

    private void Update()
    {
        if (currentState == PlayerFistState.Windup)
        {
            float prevNormWindup = GetWindupNormalized();
            windup = Mathf.Clamp(windup + Time.deltaTime * Help.Tunables.windupSpeed, 0, Help.Tunables.maxWindup - Help.Tunables.baseWindup);
            currentOffset = (windup + Help.Tunables.baseWindup) * Help.Tunables.windupOffset;
            transform.position = restingPosition.position - new Vector3((_fighter.IsFacingLeft() ? -1 : 1) * currentOffset.x, currentOffset.y, 0);

            if (prevNormWindup < Help.Tunables.windupThresholdCantCancel && GetWindupNormalized() >= Help.Tunables.windupThresholdCantCancel)
            {
                GameWizard.Instance.audioHub.PlayClip(Help.Audio.punchSuperCharged);
            }
            if (windup >= Help.Tunables.maxWindup - Help.Tunables.baseWindup)
            {
                Launch();
            }
        }
        else if(currentState == PlayerFistState.Launch)
        {
            if (Mathf.Abs(rb2d.linearVelocityX) <= Help.Tunables.speedThresholdToRetract)
            {
                Retract();
            }
        }
    }

    public void Iniialize(Fighter fighter, Transform restingPosition, Transform blockPosition)
    {
        this.restingPosition = restingPosition;
        this._fighter = fighter;
        this.blockPosition = blockPosition;
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
                highlightRenderer.enabled = false;
                windupShake.Complete();
                windupShake.Kill();
                GameWizard.Instance.audioHub.StopLoopingClip(windupAudio);
                break;
        }
        
        currentState = newState;
        switch (currentState)
        {
            case PlayerFistState.Idle:
                idleFollow = transform.DOMove(restingPosition.position, Help.Tunables.fistsIdleFollowSpeed).SetSpeedBased(true).SetEase(Ease.OutCubic);
                idleFollow.OnUpdate(() => idleFollow.ChangeEndValue(restingPosition.position, true).Restart());
                windup = 0;
                break;
                
            case PlayerFistState.Windup:
                highlightRenderer.enabled = true;
                consumed = false;
                shakeTweenComplete.Invoke();
                GameWizard.Instance.audioHub.PlayLoopingClip(windupAudio);
                break;
            
            case PlayerFistState.Launch:
                rb2d.bodyType = RigidbodyType2D.Dynamic;
                rb2d.AddForceX((_fighter.IsFacingLeft() ? -1 : 1) * (windup + Help.Tunables.baseWindup) * Help.Tunables.launchStrength, ForceMode2D.Impulse);
                GameWizard.Instance.audioHub.PlayClip(Help.Audio.punchReleased);
                break;
            
            case PlayerFistState.Retract:
                rb2d.bodyType = RigidbodyType2D.Kinematic;
                retract = transform.DOMove(restingPosition.position, Help.Tunables.retractTime).SetEase(Ease.InCubic)
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

    public void StartGrapple()
    {
        PauseFistControl();
        SwitchState(PlayerFistState.Grapple);
    }

    public void CancelGrapple()
    {
        SwitchState(PlayerFistState.Idle);
        ResumeFistControl();
    }

    private IEnumerator BlockIdle()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                blockPosition.position,
                Time.deltaTime * 5f
            );
            
            yield return null;
        }
    }

    public void StopBlock(bool fast)
    {
        blockGrowth.Complete();
        blockGrowth = artTransform.DOScale(Vector3.one, .2f).SetEase(Ease.InCubic);
        blockRotate = artTransform.DORotate(new Vector3(0, 0, 0), .2f, RotateMode.Fast);
        transform.position = blockPosition.position;
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
    }

    public Fighter GetOwner()
    {
        return _fighter;
    }

    private void PauseFistControl()
    {
        transform.position = blockPosition.position;
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
            idleFollow = transform.DOMove(restingPosition.position, Help.Tunables.fistsIdleFollowSpeed).SetSpeedBased(true).SetEase(Ease.OutCubic);
            idleFollow.OnUpdate(() => idleFollow.ChangeEndValue(restingPosition.position, true).Restart());
        }
    }

    public float GetWindupNormalized()
    {
        return (windup + Help.Tunables.baseWindup) / Help.Tunables.maxWindup;
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

    public void ForceHitboxesValue(bool value)
    {
        foreach (var fistCollider in fistColliders)
        {
            fistCollider.enabled = value;
        }
    }

    private void OnDisable()
    {
        GameWizard.Instance.audioHub.DestroyLoopingClip(windupAudio);
    }
}