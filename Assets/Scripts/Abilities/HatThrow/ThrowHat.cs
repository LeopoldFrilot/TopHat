using System;
using UnityEngine;

public class ThrowHat : HatMainAbility
{
    [SerializeField] private ThrownHat thrownHatPrefab;
    [SerializeField] private HatTossDirectionIndicator thrownHatAngleIndicatorPrefab;
    [SerializeField] private float maxAngle = 35;
    [SerializeField] private float angleChangeRate = .1f;
    [SerializeField] private float launchStrength = 20f;
    [SerializeField] private float timeTillForceReturn = 3f;
    [SerializeField] private float stunTime = .5f;
    [SerializeField] private float minStunTimeRatio = .2f;
    
    private PlayerStatus playerStatus;
    private HatInterface hatInterface;
    private HatTossDirectionIndicator spawnedDirectionIndicator;
    private bool choosingAngle;
    private int hatInactiveHandle;
    private float startHoldTime;
    private float currentAngle;
    private ThrownHat thrownHat;
    private float hatThrownTime;
    private bool hatBouncing;
    private bool hatActive;
    private bool activatedConsumed;

    protected override void Awake()
    {
        base.Awake();
        playerStatus = fighterRef.GetComponent<PlayerStatus>();
        hatInterface = fighterRef.GetComponent<HatInterface>();
    }

    private void Update()
    {
        if (hatBouncing && Time.time > hatThrownTime + timeTillForceReturn)
        {
            ForceReturn();
        }
        
        if (!choosingAngle)
        {
            return;
        }
        
        currentAngle = Mathf.Sin((Time.time - startHoldTime) * angleChangeRate) * maxAngle;
        spawnedDirectionIndicator.UpdateArrowRotation(currentAngle);
    }

    public override bool CanActivate()
    {
        if (hatActive)
        {
            return false;
        }
        
        return base.CanActivate();
    }

    public override void Activate()
    {
        base.Activate();
        
        activatedConsumed = false;
        startHoldTime = Time.time;
        choosingAngle = true;

        if (spawnedDirectionIndicator)
        {
            Destroy(spawnedDirectionIndicator.gameObject);
        }

        spawnedDirectionIndicator = Instantiate(thrownHatAngleIndicatorPrefab, fighterRef.GetHatLocation());
    }

    public override void Cancel()
    {
        base.Cancel();

        if (activatedConsumed)
        {
            return;
        }

        activatedConsumed = true;
        OnRelease();
    }

    private void OnRelease()
    {
        hatInactiveHandle = playerStatus.AddStatusEffect(StatusType.HatInactive);
        choosingAngle = false;

        if (spawnedDirectionIndicator)
        {
            Destroy(spawnedDirectionIndicator.gameObject);
        }
        
        Vector3 hatLocation = fighterRef.GetHatLocation().position;
        thrownHat = Instantiate(thrownHatPrefab, hatLocation, Quaternion.identity);
        hatActive = true;
        thrownHat.Initialize(hatInterface.GetHatStats(), fighterRef);
        thrownHat.OnFighterTriggerEntered += OnFigterTriggerEntered;
        Vector3 directionAssumingRight = new Vector3(1, Mathf.Tan(Mathf.Deg2Rad * currentAngle), 0);
        thrownHat.Launch(
            new Vector3(
                (fighterRef.IsFacingLeft() ? -1f : 1f) * directionAssumingRight.x,
                directionAssumingRight.y,
                directionAssumingRight.z), 
            launchStrength);
        hatThrownTime = Time.time;
        hatBouncing = true;
    }

    private void OnFigterTriggerEntered(Fighter hitFighter)
    {
        if (hitFighter != fighterRef)
        {
            if (!hitFighter.GetComponent<PlayerStatus>().GetActiveStatusEffects().Contains(StatusType.Invulnerable))
            {
                hitFighter.GetComponent<PlayerStatus>().AddStatusEffectForTime(StatusType.Stunned, Mathf.Clamp((Time.time - hatThrownTime)/timeTillForceReturn, minStunTimeRatio, 1) * stunTime);
            }
            ForceReturn();
        }
    }

    private void ForceReturn()
    {
        hatBouncing = false;
        OnReturn();
    }

    private void OnReturn()
    {
        if (thrownHat != null) Destroy(thrownHat.gameObject);
        if (spawnedDirectionIndicator != null) Destroy(spawnedDirectionIndicator.gameObject);
        hatActive = false;
        playerStatus.RemoveStatusEffect(hatInactiveHandle);
        hatInactiveHandle = -1;
        choosingAngle = false;
    }

    private void OnDisable()
    {
        ForceReturn();
    }
}