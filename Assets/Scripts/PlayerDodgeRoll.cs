using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgeRoll : MonoBehaviour
{
    [SerializeField] private float dodgeDistance = 10f;
    [SerializeField] private float dodgeTime = 1.5f;
    [SerializeField] private float dodgeSmoothingCoef = .3f;
    [SerializeField] private float dodgeRollCD = 1.5f;
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
        dodgeRollRoutine = StartCoroutine(DodgeRoll(dodgeRight));
        return true;
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

        if (Time.time < dodgeRollCDStartTime + dodgeRollCD)
        {
            return false;
        }
        
        return true;
    }
}