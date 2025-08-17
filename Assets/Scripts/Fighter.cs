using System;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float blockKnockbackPower = 4;
    [SerializeField] private float hitKnockbackPower = 10;
    [SerializeField] private float grappleResetPower = 20f;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject blockEffectPrefab;
    [SerializeField] private AnimationCurve dizzyDamageCurve;

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
    private PlayerDizziness playerDizziness;
    private PlayerStatus playerStatus;

    private bool AIControlled = false;
    private AIBaseComponent AIBaseComponent;
    private bool initialized = false;

    private int grappledHandle = -1;

    private NetworkedFighterController networkedFighterController;

    public void Initialize(NetworkedFighterController networkedFighterController, FightScene fightScene)
    {
        this.fightScene = fightScene;
        this.networkedFighterController = networkedFighterController;
        playerBlock = GetComponent<PlayerBlock>();
        playerMovement = GetComponent<PlayerMovement>();
        AIBaseComponent = GetComponent<AIBaseComponent>();
        playerDizziness = GetComponent<PlayerDizziness>();
        playerStatus = GetComponent<PlayerStatus>();
        inputHandler = networkedFighterController.GetComponent<InputHandler>();
        inputHandler.OnActionStarted += OnActionStarted;
        inputHandler.OnActionCancelled += OnActionCancelled;
        initialized = true;
        TriggerInitialized();
    }

    public void Restart()
    {
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
        if (!CanStartAction())
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

    public bool CanStartAction()
    {
        if (fightScene.IsInCountdown())
        {
            return false;
        }

        var effects = playerStatus.GetActiveStatusEffects();
        if (effects.Contains(StatusType.Grappled) || effects.Contains(StatusType.Stunned))
        {
            return false;
        }
        
        return true;
    }

    public bool CanMove()
    {
        return CanStartAction() && !playerBlock.IsBlocking();
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
                 SetGrappled();
                 fightScene.GetOpponent(this).SetGrappled();
                 fightScene.GetOpponent(this).StartGrappleAnimation();
             }
        }
        else
        {
            if (fist.GetCurrentState() == PlayerFistState.Launch)
            {
                bool blocked = playerBlock.IsBlocking();
                
                fist.HandleCollisionWithPlayer(this, blocked);
            
                if (blocked)
                {
                    playerBlock.CancelBlockLag();
                    Instantiate(blockEffectPrefab, blockLocation.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(hitEffectPrefab, blockLocation.position, Quaternion.identity);
                }

                playerMovement.LaunchPlayer(
                    new Vector2(1 * (fist.transform.position.x < transform.position.x ? 1 : -1), .2f) 
                    * (blocked ? blockKnockbackPower : hitKnockbackPower));
                
                playerDizziness.DealDizzyDamage(dizzyDamageCurve.Evaluate(fist.GetWindupNormalized()));
            }
        }
    }

    private void SetGrappled()
    {
        CancelGrappled();
        grappledHandle = playerStatus.AddStatusEffect(StatusType.Grappled);
    }

    private void CancelGrappled()
    {
        playerStatus.RemoveStatusEffect(grappledHandle);
        grappledHandle = -1;
    }

    public void TriggerGrappleComplete()
    {
        EventHub.TriggerPlaySoundRequested(grappleSound);
        Fighter opponent = fightScene.GetOpponent(this);
        
        opponent.GetComponent<PlayerMovement>().LaunchPlayer(new Vector2(((opponent.transform.position.x > 0) ? -1f : 1f) * 1.2f, 1f) * grappleResetPower);
        opponent.StartGrappledAnimation();
        CancelGrappled();
        opponent.CancelGrappled();
        
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
        }
    }
    
    public event Action<TurnState> OnTurnStateChanged;
    private void TriggerTurnStateChanged(TurnState newState)
    {
        OnTurnStateChanged?.Invoke(newState);
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
        var outSpawnedFist = spawnedFists[0];
        outSpawnedFist.transform.SetParent(outFistLocation);
        outSpawnedFist.transform.localScale = Vector3.one;
        outSpawnedFist.transform.localPosition = Vector3.zero;
        outSpawnedFist.PauseFistControl();
        
        var inSpawnedFist = spawnedFists[0];
        inSpawnedFist.transform.SetParent(inFistLocation);
        inSpawnedFist.transform.localScale = Vector3.one;
        inSpawnedFist.transform.localPosition = Vector3.zero;
        inSpawnedFist.PauseFistControl();
    }

    public void ResumeFistControl()
    {
        var outSpawnedFist = spawnedFists[0];
        outSpawnedFist.transform.SetParent(null);
        outSpawnedFist.ResumeFistControl();
        outSpawnedFist.transform.localScale = Vector3.one;
        
        var inSpawnedFist = spawnedFists[0];
        inSpawnedFist.transform.SetParent(null);
        inSpawnedFist.ResumeFistControl();
        inSpawnedFist.transform.localScale = Vector3.one;
    }
}