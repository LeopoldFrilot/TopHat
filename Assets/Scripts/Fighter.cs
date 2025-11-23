using System;
using System.Collections;
using System.Collections.Generic;
using MilkShake;
using ScriptableObjects;
using UnityEngine;

public enum TurnState
{
    Attacking,
    Defending
}

public enum AbilityTypes
{
    BasicAttack,
    DodgeRoll,
    Grapple,
    HatThrow,
    DashCancel,
}

public class Fighter : MonoBehaviour
{
    [SerializeField] private Transform artRoot;
    [SerializeField] private Transform inFistLocation;
    [SerializeField] private Transform outFistLocation;
    [SerializeField] private Transform blockLocation;
    [SerializeField] private Transform playerHatLocation;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject blockEffectPrefab;
    [SerializeField] private AnimationCurve dizzyDamageCurve;
    [SerializeField] private Collider2D mainCollitder;

    [Header("Animation")] 
    [SerializeField] private Animator mainBodyAnimator;
    [SerializeField] private string animatorSpeedFloatName = "speed";
    [SerializeField] private string animatorJumpBoolName = "Jump";
    [SerializeField] private string animatorGrappleTriggerName = "Grapple1";
    [SerializeField] private string animatorGrappledTriggerName = "Grapple2";
    [SerializeField] private string animatorBlockHitTriggerName = "Hatblock";
    [SerializeField] private string animatorHitTriggerName = "Hit";
    [SerializeField] private string animatorWinFaceBoolName = "Win";

    [Header("Particles")] 
    [SerializeField] private List<Transform> eyePivots = new();
    [SerializeField] private GameObject flameEyesParticles;
    
    [Header("Audio")]
    [SerializeField] AudioClip grappleSound;

    private List<PlayerFist> spawnedFists = new();
    private PlayerBlock playerBlock;
    private PlayerGrapple playerGrapple;
    private bool facingLeft;
    private TurnState currentTurnState = TurnState.Attacking;
    private PlayerMovement playerMovement;
    private InputHandler inputHandler;
    private int playerIndex = 0;
    private FightScene fightScene;
    private PlayerDizziness playerDizziness;
    private PlayerStatus playerStatus;
    private PlayerMeter playerMeter;
    private HatInterface hatInterface;
    private PlayerDashCancel playerDashCancel;
    private AutoTurnaround playerAutoTurnaround;

    private bool AIControlled = false;
    private AIBaseComponent AIBaseComponent;
    private bool initialized = false;

    private int grappledHandle = -1;
    private int dizzyStunHandle = -1;

    private float hatTime;
    private bool knockedOff = false;

    [HideInInspector] public NetworkedFighterController networkedFighterController;

    public void Initialize(NetworkedFighterController networkedFighterController, FightScene fightScene)
    {
        this.fightScene = fightScene;
        this.networkedFighterController = networkedFighterController;
        playerBlock = GetComponent<PlayerBlock>();
        playerGrapple = GetComponent<PlayerGrapple>();
        playerMovement = GetComponent<PlayerMovement>();
        AIBaseComponent = GetComponent<AIBaseComponent>();
        playerStatus = GetComponent<PlayerStatus>();
        playerMeter = GetComponent<PlayerMeter>();
        hatInterface = GetComponent<HatInterface>();
        playerDizziness = GetComponent<PlayerDizziness>();
        playerDashCancel = GetComponent<PlayerDashCancel>();
        playerAutoTurnaround = GetComponent<AutoTurnaround>();
        
        playerDizziness.OnDizzinessChanged += OnDizzinessChanged;
        playerStatus.OnStatusEffectsChanged += OnStatusEffectsChanged;
        
        inputHandler = networkedFighterController.GetComponent<InputHandler>();
        inputHandler.OnActionStarted += OnActionStarted;
        inputHandler.OnActionCancelled += OnActionCancelled;
        inputHandler.OnUpActionStarted += OnUpActionStarted;
        inputHandler.OnUpActionCancelled += OnUpActionCancelled;
        inputHandler.OnDownActionStarted += OnDownActionStarted;
        inputHandler.OnDownActionCancelled += OnDownActionCancelled;
        
        initialized = true;
        TriggerInitialized();
    }

    private void OnStatusEffectsChanged(List<StatusType> status)
    {
        if (status.Contains(StatusType.Stunned))
        {
            foreach (var spawnedFist in spawnedFists)
            {
                spawnedFist.Reset();
            }
        }
    }

    private void OnDizzinessChanged(float obj)
    {
        float normalizedDizzy = playerDizziness.GetNormalizedDizziness();
        if (normalizedDizzy >= 0.99f)
        {
            playerStatus.RemoveStatusEffect(dizzyStunHandle);
            dizzyStunHandle = playerStatus.AddStatusEffect(StatusType.Stunned);
            StartCoroutine(DizzyStun());
        }
    }

    private IEnumerator DizzyStun()
    {
        yield return new WaitForSeconds(Help.Tunables.dizzyStunTimeLength);
        EndDizzy();
    }

    private void EndDizzy()
    {
        playerStatus.RemoveStatusEffect(dizzyStunHandle);
        playerDizziness.ResetDizzy();
        dizzyStunHandle = -1; 
    }

    public void Restart()
    {
        ShowPlayer();
        hatTime = 0;

        EndDizzy();
        
        playerStatus.RemoveStatusEffect(grappledHandle);
        grappledHandle = -1;
        
        playerBlock.CancelBlockLag();
        playerGrapple.ResetGrapple();
        playerMeter.ResetMeter();

        foreach (var spawnedFist in spawnedFists)
        {
            spawnedFist.Reset();
        }
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

        playerMeter.OnMeterChanged += OnMeterChanged;
    }

    private void Update()
    {
        float speed = playerMovement.GetHorizontalVelocity();
        mainBodyAnimator.SetFloat(animatorSpeedFloatName, speed);
        if (playerMovement.IsJumping())
        {
            mainBodyAnimator.SetBool(animatorJumpBoolName, true);
        }

        if (!fightScene.IsInCountdown() && !fightScene.IsInOutro() && currentTurnState == TurnState.Defending && !IsStunned())
        {
            hatTime += Time.deltaTime;
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
        OnActionCancelled(networkedFighterController.GetPlayerIndex(this));
    }

    private void OnActionCancelled(int playerOnNetworkedController)
    {
        if (playerOnNetworkedController != networkedFighterController.GetPlayerIndex(this))
        {
            return;
        }

        foreach (var spawnedFist in spawnedFists)
        {
            if (spawnedFist.GetCurrentState() == PlayerFistState.Windup)
            {
                if (spawnedFist.GetWindupNormalized() >= Help.Tunables.windupThresholdCantCancel)
                {
                    continue;
                }
                
                spawnedFist.Launch();
                break;
            }
        }

        if (currentTurnState == TurnState.Defending)
        {
            playerBlock.HandleActionButtonCancelled();
        }
    }

    public void StartAction()
    {
        OnActionStarted(networkedFighterController.GetPlayerIndex(this));
    }

    private void OnActionStarted(int playerOnNetworkedController)
    {
        if (playerOnNetworkedController != networkedFighterController.GetPlayerIndex(this))
        {
            return;
        }
        
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

    public void CancelDownAction()
    {
        OnDownActionStarted(networkedFighterController.GetPlayerIndex(this));
    }

    private void OnDownActionCancelled(int playerOnNetworkedController)
    {
        if (playerOnNetworkedController != networkedFighterController.GetPlayerIndex(this))
        {
            return;
        }

        if (currentTurnState == TurnState.Defending)
        {
            hatInterface.OnMovementActionCancelled();
        }
    }

    public void StartDownAction()
    {
        OnDownActionStarted(networkedFighterController.GetPlayerIndex(this));
    }

    private void OnDownActionStarted(int playerOnNetworkedController)
    {
        if (playerOnNetworkedController != networkedFighterController.GetPlayerIndex(this))
        {
            return;
        }
        
        if (fightScene.IsInCountdown() || fightScene.IsInOutro())
        {
            return;
        }

        if (currentTurnState == TurnState.Attacking)
        {
            playerDashCancel.TryStartDashCancel(!playerMovement.IsHoldingLeft());
        }
        else
        {
            if (!CanStartAction())
            {
                return;
            }
            
            hatInterface.OnMovementActionStarted();
        }
    }

    public void CancelUpAction()
    {
        OnDownActionStarted(networkedFighterController.GetPlayerIndex(this));
    }

    private void OnUpActionCancelled(int playerOnNetworkedController)
    {
        if (playerOnNetworkedController != networkedFighterController.GetPlayerIndex(this))
        {
            return;
        }

        if (currentTurnState == TurnState.Attacking)
        {
            
        }
        else
        {
            hatInterface.OnMainActionCancelled();
        }
    }

    public void StartUpAction()
    {
        OnDownActionStarted(networkedFighterController.GetPlayerIndex(this));
    }

    private void OnUpActionStarted(int playerOnNetworkedController)
    {
        if (playerOnNetworkedController != networkedFighterController.GetPlayerIndex(this))
        {
            return;
        }
        
        if (!CanStartAction())
        {
            return;
        }

        if (currentTurnState == TurnState.Attacking)
        {
            playerGrapple.TryToGrapple();
        }
        else
        {
            hatInterface.OnMainActionStarted();
        }
    }

    public bool CanStartAction()
    {
        if (fightScene.IsInCountdown() || fightScene.IsInOutro())
        {
            return false;
        }

        var effects = playerStatus.GetActiveStatusEffects();
        if (effects.Contains(StatusType.Grappled) || effects.Contains(StatusType.Stunned) || effects.Contains(StatusType.AbilityLag))
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

    public void ForceHitboxesValueOn()
    {
        foreach (var spawnedFist in spawnedFists)
        {
            spawnedFist.ForceHitboxesValue(true);
        }
    }

    public void ForceHitboxesValueOff()
    {
        foreach (var spawnedFist in spawnedFists)
        {
            spawnedFist.ForceHitboxesValue(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.parent)
        {
            return;
        }
        
        PlayerFist fist = other.transform.parent.GetComponent<PlayerFist>();
        if (fist == null)
        {
            fist = other.transform.root.GetComponent<PlayerFist>();
        }
        
        if (fist == null || spawnedFists.Contains(fist))
        {
            return;
        }

        bool isInvul = playerStatus.GetActiveStatusEffects().Contains(StatusType.Invulnerable);
        if (fist.GetOwner().IsGrappling())
        {
            if (isInvul)
            {
                return;
            }
            if (GetTurnState() == TurnState.Defending)
            {
                TriggerKnockOff();
            }
            foreach (var spawnedFist in spawnedFists)
            {
                spawnedFist.Reset();
            }
            
            SetGrappled();
            
            return;
        }
        
        if (fist.GetCurrentState() == PlayerFistState.Launch && !fist.IsConsumed())
        {
            bool facingOpponent = playerAutoTurnaround.IsFacingOpponent(fist.GetOwner());
            bool willKnockOff = currentTurnState == TurnState.Defending && IsStunned() && !knockedOff && !isInvul;
            bool blocked = isInvul || (playerBlock.IsBlocking() && !willKnockOff && facingOpponent);
            bool wasPerfectBlock = playerBlock.IsPerfectBlock() && !willKnockOff && facingOpponent;
            
            if (blocked)
            {
                StartBlockAnimation();
            }
            else
            {
                StartHitAnimation();
            }

            if (willKnockOff)
            {
                TriggerKnockOff();
                fist.GetOwner().ChangeMeter(Help.Tunables.meterForHit_Hard);
                GameWizard.Instance.hitStopManager.AddStop(Help.Tunables.knockOffHitstop, Help.Tunables.swapTimeRecoveryRate);
            }
                
            fist.HandleCollisionWithPlayer(this, blocked);


            if (isInvul)
            {
                return;
            }

            if (!willKnockOff)
            {
                float normWindup = fist.GetWindupNormalized();
                if (blocked)
                {
                    Instantiate(blockEffectPrefab, blockLocation.position, Quaternion.identity);
                    if (!wasPerfectBlock)
                    {
                        GameWizard.Instance.hitStopManager.AddStop(Help.Tunables.blockHitstop);
                        Shaker.ShakeAll(Help.Tunables.blockCameraShake);
                        if (normWindup >= Help.Tunables.hardHitThreshold)
                        {
                            fist.GetOwner().ChangeMeter(Help.Tunables.meterForHitBlock_Hard);
                            playerDizziness.DealDizzyDamage(dizzyDamageCurve.Evaluate(normWindup * Help.Tunables.blockDizzyDamageReduction));
                            playerMovement.LaunchPlayer(
                                new Vector2(1 * (fist.transform.position.x < transform.position.x ? 1 : -1), .2f) * Help.Tunables.blockKnockbackPower);

                            if (Mathf.Approximately(normWindup, 1))
                            {
                                GameWizard.Instance.hitStopManager.AddStop(Help.Tunables.blockStrongestHitstop);
                                playerBlock.EndBlock();
                            }
                            else
                            {
                                GameWizard.Instance.hitStopManager.AddStop(Help.Tunables.blockStrongHitstop);
                            }
                        }
                        else
                        {
                            GameWizard.Instance.hitStopManager.AddStop(Help.Tunables.blockHitstop);
                        }
                        
                        playerMeter.ChangeMeter(Help.Tunables.meterForBlock);
                    }
                    else
                    {
                        playerBlock.CancelBlockLag();
                        GameWizard.Instance.hitStopManager.AddStop(Help.Tunables.parryHitstop);
                        playerMeter.ChangeMeter(Help.Tunables.meterForParry);
                    }
                }
                else
                {
                    Instantiate(hitEffectPrefab, blockLocation.position, Quaternion.identity);
                    playerDizziness.DealDizzyDamage(dizzyDamageCurve.Evaluate(normWindup));
                    playerMovement.LaunchPlayer(
                        new Vector2(1 * (fist.transform.position.x < transform.position.x ? 1 : -1), .2f) * Help.Tunables.hitKnockbackPower);

                    if (normWindup >= Help.Tunables.hardHitThreshold)
                    {
                        fist.GetOwner().ChangeMeter(Help.Tunables.meterForHit_Hard);
                        Shaker.ShakeAll(Help.Tunables.hardPunchCameraShake);
                        if (Mathf.Approximately(normWindup, 1))
                        {
                            GameWizard.Instance.hitStopManager.AddStop(Help.Tunables.hitStrongestHitstop, Help.Tunables.hitTimeRecoveryRate);
                            playerDizziness.DealDizzyDamage(Help.Tunables.maxDizziness);
                        }
                        else
                        {
                            GameWizard.Instance.hitStopManager.AddStop(Help.Tunables.hitStrongHitstop, Help.Tunables.hitTimeRecoveryRate);
                        }
                    }
                    else
                    {
                        fist.GetOwner().ChangeMeter(Help.Tunables.meterForHit);
                        Shaker.ShakeAll(Help.Tunables.smallPunchCameraShake);
                        GameWizard.Instance.hitStopManager.AddStop(Help.Tunables.punchHitstop, Help.Tunables.hitTimeRecoveryRate);
                    }
                } 
            }
        }
    }

    private bool IsGrappling()
    {
        return playerGrapple.IsGrappling();
    }

    private void OnMeterChanged(float meter)
    {
        if (currentTurnState ==  TurnState.Attacking)
        {
            ToggleFlameEyes(meter >= Help.Tunables.meterRequirementGrapple);
        }
        else
        {
            ToggleFlameEyes(false);
        }
    }

    private void ToggleFlameEyes(bool On)
    {
        foreach (Transform eyePivot in eyePivots)
        {
            if (On && eyePivot.childCount == 0)
            {
                Instantiate(flameEyesParticles, eyePivot);
            }
            else if (!On && eyePivot.childCount > 0)
            {
                for (int i = 0; i < eyePivot.childCount; i++)
                {
                    Destroy(eyePivot.GetChild(i).gameObject);
                }
            }
        }
    }

    public Action OnKnockOff;

    private void TriggerKnockOff()
    {
        knockedOff = true;
        OnKnockOff?.Invoke();
    }

    public void SetGrappled()
    {
        CancelGrappled();
        grappledHandle = playerStatus.AddStatusEffect(StatusType.Grappled);
    }

    private void CancelGrappled()
    {
        playerStatus.RemoveStatusEffect(grappledHandle);
        playerGrapple.ResetGrapple();
        grappledHandle = -1;
    }

    public void TriggerGrappleComplete()
    {
        EventHub.TriggerPlaySoundRequested(grappleSound);
        Fighter opponent = fightScene.GetOpponent(this);
        CancelGrappled();
        if (opponent.IsGrappled())
        {
            opponent.GetComponent<PlayerMovement>().LaunchPlayer(new Vector2(((opponent.transform.position.x > 0) ? -1f : 1f) * 1.2f, 1f) * Help.Tunables.grappleResetPower);
            opponent.StartGrappledAnimation();
            opponent.CancelGrappled();
        }
    }

    private bool IsGrappled()
    {
        return playerStatus.GetActiveStatusEffects().Contains(StatusType.Grappled);
    }

    public void StartGrappleAnimation()
    {
        mainBodyAnimator.SetTrigger(animatorGrappleTriggerName);
    }

    private void StartGrappledAnimation()
    {
        mainBodyAnimator.SetTrigger(animatorGrappledTriggerName);
    }

    public void StartBlockAnimation()
    {
        mainBodyAnimator.SetTrigger(animatorBlockHitTriggerName);
    }

    public void StartHitAnimation()
    {
        mainBodyAnimator.SetTrigger(animatorHitTriggerName);
    }

    public void ToggleWinFaceAnimation(bool newValue)
    {
        mainBodyAnimator.SetBool(animatorWinFaceBoolName, newValue);
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
            SetHat(newState == TurnState.Attacking ? null : fightScene.GetHat());

            currentTurnState = newState;
            EndDizzy();
            TriggerTurnStateChanged(currentTurnState);
        }
    }
    
    public event Action<TurnState> OnTurnStateChanged;
    private void TriggerTurnStateChanged(TurnState newState)
    {
        if (currentTurnState == TurnState.Defending)
        {
            knockedOff = false;
        }
        
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
        outSpawnedFist.StartGrapple();
        
        var inSpawnedFist = spawnedFists[1];
        inSpawnedFist.transform.SetParent(inFistLocation);
        inSpawnedFist.transform.localScale = Vector3.one;
        inSpawnedFist.transform.localPosition = Vector3.zero;
        inSpawnedFist.StartGrapple();
    }

    public void ResumeFistControl()
    {
        var outSpawnedFist = spawnedFists[0];
        outSpawnedFist.transform.SetParent(null);
        outSpawnedFist.transform.localScale = Vector3.one;
        outSpawnedFist.CancelGrapple();
        
        var inSpawnedFist = spawnedFists[1];
        inSpawnedFist.transform.SetParent(null);
        inSpawnedFist.transform.localScale = Vector3.one;
        inSpawnedFist.CancelGrapple();
    }

    public bool IsStunned()
    {
        return playerStatus.GetActiveStatusEffects().Contains(StatusType.Stunned);
    }

    public float GetHatTime()
    {
        return hatTime;
    }

    public Vector3 ClampToFightPosition(Vector3 newPosition)
    {
        return fightScene.ClampToGameplayBounds(newPosition);
    }

    public void ChangeMeter(float delta)
    {
        playerMeter.ChangeMeter(delta);
    }

    public float GetMeter()
    {
        return playerMeter.GetMeter();
    }

    public bool AreFistsOfState(List<PlayerFistState> validStates)
    {
        foreach (var fist in spawnedFists)
        {
            if (!validStates.Contains(fist.GetCurrentState()) )
            {
                return false;
            }
        }
        
        return true;
    }

    public bool IsAFistsOfState(List<PlayerFistState> validStates)
    {
        foreach (var fist in spawnedFists)
        {
            if (validStates.Contains(fist.GetCurrentState()) )
            {
                return true;
            }
        }
        
        return false;
    }

    public void SetColliderEnabled(bool newEnabled)
    {
        mainCollitder.enabled = newEnabled;
    }

    public void SetHat(PlayerHat hat)
    {
        hatInterface.SetHat(hat);
    }

    public Collider2D GetMainCollider()
    {
        return mainCollitder;
    }

    public int GetPlayerIndex()
    {
        return playerIndex;
    }

    public void HidePlayer()
    {
        artRoot.gameObject.SetActive(false);
    }

    public void ShowPlayer()
    {
        artRoot.gameObject.SetActive(true);
    }

    public Transform GetArtRoot()
    {
        return artRoot;
    }
}