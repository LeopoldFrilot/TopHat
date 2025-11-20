using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgeRoll : MonoBehaviour
{
    [SerializeField] private Collider2D mainCollider;
    
    private bool isRolling;
    private Fighter fighterRef;
    private PlayerStatus fighterStatus;
    private Coroutine dodgeRollRoutine;
    private int dodgeRollEffectHandle;
    private int dodgeRollInvulEffectHandle;
    private double dodgeRollCDStartTime;

    private void Awake()
    {
        fighterRef = GetComponent<Fighter>();
        fighterStatus = fighterRef.GetComponent<PlayerStatus>();
    }

    public bool TryStartDodgeRoll(bool dodgeRight)
    {
        if (!CanDodgeRoll())
        {
            return false;
        }
        
        mainCollider.enabled = false;
        fighterRef.ChangeMeter(-Help.Tunables.meterRequirementDodgeRoll);
        dodgeRollRoutine = StartCoroutine(DodgeRoll(dodgeRight));
        return true;
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
        mainCollider.enabled = true;
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

        if (fighterRef.GetMeter() < Help.Tunables.meterRequirementDodgeRoll)
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