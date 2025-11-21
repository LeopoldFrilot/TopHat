using System.Collections;
using UnityEngine;

public class PlayerDashCancel : MonoBehaviour
{
    private Fighter fighterRef;
    private bool isRolling;
    private PlayerStatus fighterStatus;
    private PlayerBlock fighterBlock;
    private Coroutine dashCancelRoutine;
    private int dashCancelEffectHandle;
    private int dashCancelInvulEffectHandle;
    private double dashCancelCDStartTime;

    private void Awake()
    {
        fighterRef = GetComponent<Fighter>();
        fighterStatus = fighterRef.GetComponent<PlayerStatus>();
        fighterBlock = fighterRef.GetComponent<PlayerBlock>();
    }

    public bool TryStartDashCancel(bool dodgeRight)
    {
        if (!CanDashCancel())
        {
            return false;
        }
        
        var spawnedFists = fighterRef.GetSpawnedFists();
        foreach (var spawnedFist in spawnedFists)
        {
            spawnedFist.Reset();
        }

        fighterBlock.CancelBlockLag();
        fighterStatus.RemoveStatusEffectsOfType(StatusType.AbilityLag);
        fighterRef.SetColliderEnabled(false);
        fighterRef.ChangeMeter(-Help.Tunables.meterRequirementDashCancel);
        dashCancelRoutine = StartCoroutine(DashCancel(dodgeRight));
        
        return true;
    }

    private IEnumerator DashCancel(bool dashRight)
    {
        Vector3 dashStartPosition = transform.position;
        Vector3 dashEndPosition = dashStartPosition + new Vector3(
            (dashRight ? 1 : -1) * Help.Tunables.dashCancelDistance, 0, 0);
        dashEndPosition = fighterRef.ClampToFightPosition(dashEndPosition);
        dashCancelEffectHandle = fighterStatus.AddStatusEffect(StatusType.AbilityLag);
        dashCancelInvulEffectHandle = fighterStatus.AddStatusEffect(StatusType.Invulnerable);
        double timeStart = Time.time;
        double timeEnd = Time.time + Help.Tunables.dashCancelTime;
        while (Time.time <= timeEnd)
        {
            double time = Time.time - timeStart;
            double maxTime = Help.Tunables.dashCancelTime;
            fighterRef.transform.position = Vector3.Lerp(dashStartPosition, dashEndPosition, Mathf.Pow((float)(time/maxTime), Help.Tunables.dashCancelSmoothingCoef));
            yield return null;
        }
        fighterRef.transform.position = dashEndPosition;
        EndDashCancel();
    }

    private void EndDashCancel()
    {
        fighterRef.SetColliderEnabled(true);
        StopCoroutine(dashCancelRoutine);
        dashCancelRoutine = null;
        fighterStatus.RemoveStatusEffect(dashCancelEffectHandle);
        fighterStatus.RemoveStatusEffect(dashCancelInvulEffectHandle);
        dashCancelEffectHandle = -1;
        dashCancelInvulEffectHandle = -1;
    }

    private bool CanDashCancel()
    {
        if (dashCancelRoutine != null)
        {
            return false;
        }

        if (fighterRef.GetMeter() < Help.Tunables.meterRequirementDashCancel)
        {
            return false;
        }

        var effects = fighterStatus.GetActiveStatusEffects();
        if (effects.Contains(StatusType.Stunned))
        {
            return false;
        }
        
        return true;
    }
}