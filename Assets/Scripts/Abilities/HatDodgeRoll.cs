using System;
using System.Collections;
using UnityEngine;

public class HatDodgeRoll : HatMovementAbility
{
    [SerializeField] float dodgeDistance = 7f;
    [SerializeField] float dodgeTime = .3f;
    [SerializeField] float dodgeSmoothingCoef = .2f;
    [SerializeField] float dodgeRollCD = 0.5f;
    
    private bool isRolling;
    private PlayerStatus fighterStatus;
    private PlayerBlock fighterBlock;
    private PlayerMovement fighterMovement;
    private Coroutine dodgeRollRoutine;
    private int dodgeRollEffectHandle;
    private int dodgeRollInvulEffectHandle;
    private double dodgeRollCDStartTime;

    protected override void Awake()
    {
        base.Awake();
        fighterStatus = fighterRef.GetComponent<PlayerStatus>();
        fighterBlock = fighterRef.GetComponent<PlayerBlock>();
        fighterMovement = fighterRef.GetComponent<PlayerMovement>();
    }

    public override void Activate()
    {
        base.Activate();

        var spawnedFists = fighterRef.GetSpawnedFists();
        foreach (var spawnedFist in spawnedFists)
        {
            spawnedFist.Reset();
        }

        StartDodgeRoll(!fighterMovement.IsHoldingLeft());
        fighterBlock.CancelBlockLag();
    }

    public override bool CanActivate()
    {
        if (!CanDodgeRoll())
        {
            return false;
        }
        
        return base.CanActivate();
    }

    private void StartDodgeRoll(bool dodgeRight)
    {
        fighterRef.SetColliderEnabled(false);
        fighterRef.ChangeMeter(-GetMeterCost());
        dodgeRollRoutine = StartCoroutine(DodgeRoll(dodgeRight));
    }

    private IEnumerator DodgeRoll(bool dodgeRight)
    {
        Vector3 rollStartPosition = transform.position;
        Vector3 rollEndPosition = rollStartPosition + new Vector3(
            (dodgeRight ? 1 : -1) * dodgeDistance, 0, 0);
        rollEndPosition = fighterRef.ClampToFightPosition(rollEndPosition);
        dodgeRollEffectHandle = fighterStatus.AddStatusEffect(StatusType.AbilityLag);
        dodgeRollInvulEffectHandle = fighterStatus.AddStatusEffect(StatusType.Invulnerable);
        double timeStart = Time.time;
        double timeEnd = Time.time + dodgeTime;
        while (Time.time <= timeEnd)
        {
            double time = Time.time - timeStart;
            double maxTime = dodgeTime;
            fighterRef.transform.position = Vector3.Lerp(rollStartPosition, rollEndPosition, Mathf.Pow((float)(time/maxTime), dodgeSmoothingCoef));
            yield return null;
        }
        fighterRef.transform.position = rollEndPosition;
        StartCooldown();
        EndDodgeRoll();
    }

    private void EndDodgeRoll()
    {
        fighterRef.SetColliderEnabled(true);
        StopCoroutine(dodgeRollRoutine);
        dodgeRollRoutine = null;
        fighterStatus.RemoveStatusEffect(dodgeRollEffectHandle);
        fighterStatus.RemoveStatusEffect(dodgeRollInvulEffectHandle);
        dodgeRollEffectHandle = -1;
        dodgeRollInvulEffectHandle = -1;
    }

    private void StartCooldown()
    {
        dodgeRollCDStartTime = Time.time;
    }

    private bool CanDodgeRoll()
    {
        if (dodgeRollRoutine != null)
        {
            return false;
        }

        if (!HasMeterForAbility())
        {
            return false;
        }

        if (Time.time < dodgeRollCDStartTime + dodgeRollCD)
        {
            return false;
        }
        
        return true;
    }
}