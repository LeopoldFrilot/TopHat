using System.Collections;
using UnityEngine;

public class PlayerDodgeRoll : HatMovementAbility
{
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
            (dodgeRight ? 1 : -1) * Help.Tunables.dodgeDistance, 0, 0);
        rollEndPosition = fighterRef.ClampToFightPosition(rollEndPosition);
        dodgeRollEffectHandle = fighterStatus.AddStatusEffect(StatusType.AbilityLag);
        dodgeRollInvulEffectHandle = fighterStatus.AddStatusEffect(StatusType.Invulnerable);
        double timeStart = Time.time;
        double timeEnd = Time.time + Help.Tunables.dodgeTime;
        while (Time.time <= timeEnd)
        {
            double time = Time.time - timeStart;
            double maxTime = Help.Tunables.dodgeTime;
            fighterRef.transform.position = Vector3.Lerp(rollStartPosition, rollEndPosition, Mathf.Pow((float)(time/maxTime), Help.Tunables.dodgeSmoothingCoef));
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

        if (Time.time < dodgeRollCDStartTime + Help.Tunables.dodgeRollCD)
        {
            return false;
        }
        
        return true;
    }
}