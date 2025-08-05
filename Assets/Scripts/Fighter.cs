using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum TurnState
{
    Attacking,
    Defending
}

public class Fighter : MonoBehaviour
{
    [SerializeField] private Transform artRoot;
    [SerializeField] private Transform inFistLocation;
    [SerializeField] private Transform outFistLocation;
    [SerializeField] private Transform blockLocation;
    [SerializeField] private Transform playerHatLocation;
    [SerializeField] private Collider2D crosslineTrigger;
    [SerializeField] private int pointsForGrapple = 5;
    [SerializeField] private float blockKnockbackPower = 4;
    [SerializeField] private float hitKnockbackPower = 10;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject blockEffectPrefab;

    [Header("Animation")] 
    [SerializeField] private Animator mainBodyAnimator;
    [SerializeField] private string animatorSpeedFloatName = "speed";
    [SerializeField] private string animatorJumpBoolName = "Jump";
    [SerializeField] private string animatorGrappleTriggerName = "Grapple1";
    [SerializeField] private string animatorGrappledTriggerName = "Grapple2";
    
    [Header("Audio")]
    [SerializeField] AudioClip grappleSound;

    private List<PlayerFist> spawnedFists = new();
    private PlayerBlock playerBlock;
    private bool facingLeft;
    private TurnState currentTurnState = TurnState.Attacking;
    private PlayerMovement playerMovement;
    private InputHandler inputHandler;
    private int playerIndex = 0;
    private FightScene fightScene;

    private int hitsThisRound = 0;
    private int blocksThisRound = 0;
    private int landedBlowsThisGame = 0;
    private int launches = 0;
    private bool AIControlled = false;
    private AIBaseComponent AIBaseComponent;
    private bool initialized = false;

    private NetworkedFighterController networkedFighterController;

    public void Initialize(NetworkedFighterController networkedFighterController, FightScene fightScene)
    {
        this.fightScene = fightScene;
        this.networkedFighterController = networkedFighterController;
        playerBlock = GetComponent<PlayerBlock>();
        playerMovement = GetComponent<PlayerMovement>();
        AIBaseComponent = GetComponent<AIBaseComponent>();
        inputHandler = networkedFighterController.GetComponent<InputHandler>();
        inputHandler.OnActionStarted += OnActionStarted;
        inputHandler.OnActionCancelled += OnActionCancelled;
        initialized = true;
        TriggerInitialized();
    }

    public Action OnInitialized;
    public void TriggerInitialized()
    {
        OnInitialized?.Invoke();
    }

    private void Start()
    {
        var inSpawnedFist = inFistLocation.GetComponentInChildren<PlayerFist>();
        inSpawnedFist.transform.SetParent(null);
        inSpawnedFist.transform.localScale = Vector3.one;
        inSpawnedFist.Iniialize(this, inFistLocation, blockLocation);
        
        var outSpawnedFist = outFistLocation.GetComponentInChildren<PlayerFist>();
        outSpawnedFist.transform.SetParent(null);
        outSpawnedFist.transform.localScale = Vector3.one;
        outSpawnedFist.Iniialize(this, outFistLocation, blockLocation);

        spawnedFists = new List<PlayerFist>{ outSpawnedFist, inSpawnedFist };
    }

    private void Update()
    {
        float speed = playerMovement.GetHorizontalVelocity();
        mainBodyAnimator.SetFloat(animatorSpeedFloatName, speed);
        if (playerMovement.IsJumping())
        {
            mainBodyAnimator.SetBool(animatorJumpBoolName, true);
        }
    }

    public bool IsAIControlled()
    {
        return AIControlled;
    }

    public Transform GetHatLocation()
    {
        return playerHatLocation;
    }

    private void StartRound()
    {
        hitsThisRound = 0;
        blocksThisRound = 0;
        launches = 0;
    }
    
    public List<PlayerFist> GetSpawnedFists() => spawnedFists;

    public void SetPlayerIndex(int inPlayerIndex)
    {
        playerIndex = inPlayerIndex;
    }

    public void CancelAction()
    {
        OnActionCancelled(0);
    }

    private void OnActionCancelled(float duration)
    {
        foreach (var spawnedFist in spawnedFists)
        {
            if (spawnedFist.GetCurrentState() == PlayerFistState.Windup)
            {
                spawnedFist.Launch();
                break;
            }
        }
    }

    public void StartAction()
    {
        OnActionStarted();
    }

    private void OnActionStarted()
    {
        if (fightScene.IsInCountdown())
        {
            return;
        }
        
        if (currentTurnState == TurnState.Attacking)
        {
            foreach (var spawnedFist in spawnedFists)
            {
                if (spawnedFist.GetCurrentState() == PlayerFistState.Idle)
                {
                    spawnedFist.StartWindup();
                    break;
                }
            }
        }
        else
        {
            if (!playerMovement.IsJumping())
            {
                playerBlock.TryToBLock();
            }
        }
    }

    public void FaceLeft()
    {
        artRoot.localScale = new Vector3(-1 * Mathf.Abs(artRoot.localScale.x), artRoot.localScale.y, artRoot.localScale.z);
        foreach (var spawnedFist in spawnedFists)
        {
            spawnedFist.GetComponent<PlayerFistUI>().FaceRight(false);
        }
        facingLeft = true;
    }

    public void FaceRight()
    {
        artRoot.localScale = new Vector3(Mathf.Abs(artRoot.localScale.x), artRoot.localScale.y, artRoot.localScale.z);
        foreach (var spawnedFist in spawnedFists)
        {
            spawnedFist.GetComponent<PlayerFistUI>().FaceRight(true);
        }
        facingLeft = false;
    }

    public bool IsFacingLeft()
    {
        return facingLeft;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerFist fist = other.transform.root.GetComponent<PlayerFist>();
        if (fist == null || spawnedFists.Contains(fist))
        {
            return;
        }
        if (currentTurnState == TurnState.Attacking)
        {
             if (fist.GetOwner().IsBlocking())
             {
                 fist.GetOwner().AddPoints(pointsForGrapple);
                 EventHub.TriggerPlaySoundRequested(grappleSound);
                 EventHub.TriggerPlayerGrappled(this);
                 fightScene.GetOpponent(this).StartGrappleAnimation();
                 StartGrappledAnimation();
             }
        }
        else
        {
            if (fist.GetCurrentState() == PlayerFistState.Launch)
            {
                bool hitCrossline = false;
                List<Collider2D> otherColliders = new List<Collider2D>();
                other.Overlap(otherColliders);
                if (otherColliders.Contains(crosslineTrigger))
                {
                    hitCrossline = true;
                }
            
                PlayerFistState state = fist.GetCurrentState();
                bool blocked = playerBlock.IsBlocking();
                if (!hitCrossline)
                {
                    fist.HandleCollisionWithPlayer(this, blocked);
            
                    if (blocked)
                    {
                        playerBlock.CancelBlockLag();
                        blocksThisRound++;
                        Instantiate(blockEffectPrefab, blockLocation.position, Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(hitEffectPrefab, blockLocation.position, Quaternion.identity);
                    }
                }

                hitsThisRound++;
                EventHub.TriggerFistConnected(this, fist, state);

                if (!hitCrossline)
                {
                    playerMovement.LaunchPlayer(
                        new Vector2(1 * (fist.transform.position.x < transform.position.x ? 1 : -1), .2f) 
                        * (blocked ? blockKnockbackPower : hitKnockbackPower));
                }
            }
        }
    }

    private void StartGrappleAnimation()
    {
        mainBodyAnimator.SetTrigger(animatorGrappleTriggerName);
    }

    private void StartGrappledAnimation()
    {
        mainBodyAnimator.SetTrigger(animatorGrappledTriggerName);
    }

    public bool IsBlocking()
    {
        return playerBlock.IsBlocking();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            playerMovement.Land();
            mainBodyAnimator.SetBool(animatorJumpBoolName, false);
        }
        
    }

    public void SwitchTurnState(TurnState newState)
    {
        if (currentTurnState != newState)
        {
            currentTurnState = newState;
            TriggerTurnStateChanged(currentTurnState);
            StartRound();
        }
    }
    
    public event Action<TurnState> OnTurnStateChanged;
    private void TriggerTurnStateChanged(TurnState newState)
    {
        OnTurnStateChanged?.Invoke(newState);
    }

    public bool CanMove()
    {
        return !playerBlock.IsBlocking() && !fightScene.IsInCountdown();
    }

    public void AddPoints(int amount)
    {
        landedBlowsThisGame += amount;
        EventHub.TriggerPlayerEarnedPoints(this);
    }

    public int GetHitsThisRound()
    {
        return hitsThisRound;
    }

    public void SwapTurnState()
    {
        if (currentTurnState == TurnState.Attacking)
        {
            SwitchTurnState(TurnState.Defending);
        }
        else
        {
            SwitchTurnState(TurnState.Attacking);
        }
    }

    public void Restart()
    {
        landedBlowsThisGame = 0;
        EventHub.TriggerPlayerEarnedPoints(this);
        StartRound();
    }

    public int GetPointsThisGame()
    {
        return landedBlowsThisGame;
    }

    public TurnState GetTurnState()
    {
        return currentTurnState;
    }

    public void SetAIControlled(bool newValue)
    {
        AIControlled = newValue;
        if (AIControlled)
        {
            AIBaseComponent.SetActive(true);
            AIBaseComponent.Activate();
        }
        else
        {
            AIBaseComponent.Deactivate();
            AIBaseComponent.SetActive(false);
        }
        
        inputHandler.SetActive(!AIControlled);
    }

    public void AddLaunches(int i)
    {
        launches += i;
    }

    public int GetLaunchCount()
    {
        return launches;
    }

    public void InstallAIModules(GameObject attackModule, GameObject defenseModule, GameObject movementModule)
    {
        AIBaseComponent.InstallAIModules(attackModule, defenseModule, movementModule);
    }

    public InputHandler GetInputHandler()
    {
        return inputHandler;
    }

    public bool IsInitioalized()
    {
        return initialized;
    }

    public void PauseFistControl()
    {
        foreach (var fist in spawnedFists)
        {
            fist.PauseFistControl();
        }
    }

    public void ResumeFistControl()
    {
        foreach (var fist in spawnedFists)
        {
            fist.ResumeFistControl();
        }
    }
}