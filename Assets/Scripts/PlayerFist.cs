using System;
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

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerFist : MonoBehaviour
{
    [SerializeField] private float maxWindup = 10f;
    [SerializeField] private float windupSpeed = 10f;
    [SerializeField] private float launchStrength = 5f;
    [SerializeField] private float retractTime = .5f;
    [SerializeField] private float baseWindup = 1f;
    [SerializeField] private float idleFollowSpeed = 5f;
    [SerializeField] private Vector2 windupOffset = new Vector2(1f, 0);
    [SerializeField] private Transform artTransform;
    [SerializeField] private List<Collider2D> fistColliders = new List<Collider2D>();
    
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
    private Player player;
    private Tweener idleFollow = null;
    private Tweener windupShake = null;
    private Tweener retract = null;
    private Tweener block = null;
    private Tweener blockGrowth = null;
    private TweenCallback shakeTweenComplete;
    private AudioSource windupAudio;

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
            windupShake = artTransform.DOShakePosition(.1f, .003f * windup * windup).SetRelative()
                .OnComplete(shakeTweenComplete);
        };
    }

    private void Update()
    {
        if (currentState == PlayerFistState.Windup)
        {
            windup = Mathf.Clamp(windup + Time.deltaTime * windupSpeed, 0, maxWindup);
            currentOffset = windup * windupOffset;
            transform.position = restingPosition.position - new Vector3((player.IsFacingLeft() ? -1 : 1) * currentOffset.x, currentOffset.y, 0);
        }
        else if(currentState == PlayerFistState.Launch)
        {
            if (Mathf.Abs(rb2d.linearVelocityX) <= .4f)
            {
                Retract();
            }
        }
    }

    public void Iniialize(Player player, Transform restingPosition, Transform blockPosition)
    {
        this.restingPosition = restingPosition;
        this.player = player;
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
                shakeTweenComplete.Invoke();
                windupAudio.Play();
                break;
            
            case PlayerFistState.Launch:
                rb2d.bodyType = RigidbodyType2D.Dynamic;
                rb2d.AddForceX((player.IsFacingLeft() ? -1 : 1) * (windup + baseWindup) * launchStrength, ForceMode2D.Impulse);
                EventHub.TriggerPlaySoundRequested(fistReleaseSound, .5f);
                GetOwner().AddLaunches(1);
                break;
            
            case PlayerFistState.Retract:
                rb2d.bodyType = RigidbodyType2D.Kinematic;
                retract = transform.DOMove(restingPosition.position, retractTime).SetEase(Ease.InCubic)
                    .OnComplete(()=>SwitchState(PlayerFistState.Idle));
                if (GetOwner().GetLaunchCount() >= 3)
                {
                    EventHub.TriggerTurnEnded(GetOwner());
                }
                break;
        }

        foreach (var fistCollider in fistColliders)
        {
            fistCollider.enabled = currentState is PlayerFistState.Block or PlayerFistState.Launch;
        }
    }


    public void StartWindup()
    {
        if (GetOwner().GetLaunchCount() < 3)
        {
            SwitchState(PlayerFistState.Windup);
        }
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
        block = transform.DOMove(blockPosition.position, .2f).SetEase(Ease.InCubic);
        //blockGrowth = artTransform.DOScale(Vector3.one * 1.5f, .2f).SetEase(Ease.InCubic);
    }

    public void StopBlock(bool fast)
    {
        blockGrowth.Complete();
        //blockGrowth = artTransform.DOScale(Vector3.one, .2f).SetEase(Ease.InCubic);
        if (block.IsActive())
        {
            block.OnComplete(() =>
            {
                blockGrowth.Complete();
                SwitchState(PlayerFistState.Idle);
            });
            
            if (fast)
            {
                block.Complete();
            }
        }
        else
        {
            blockGrowth.Complete();
            SwitchState(PlayerFistState.Idle);
        }
    }

    public void HandleCollisionWithPlayer(Player player1, bool wasBlocked)
    {
       rb2d.linearVelocity = Vector2.zero;
       //artTransform.DOShakeScale(.5f);
       if (!wasBlocked)
       {
           player.AddPoints(1);
       }
       EventHub.TriggerPlaySoundRequested(wasBlocked ? fistBlockedSound : fistHitSound);
    }

    public Player GetOwner()
    {
        return player;
    }
}