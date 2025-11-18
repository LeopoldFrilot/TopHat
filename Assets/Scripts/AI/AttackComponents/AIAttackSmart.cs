using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIAttackSmart : AIAttackModule
{
    [SerializeField] private float spamDistance = 5f;
    [SerializeField] private float neutralDistance = 12f;
    private AutoTurnaround autoTurnaround;
    private double lastLaunchTime = 0;
    private double lastCancelTime = 0;
    private float randomlaunchTimeDiff = 0;
    private float randomCancelTimeDiff = 0;
    
    private void Update()
    {
        if (!IsActive())
        {
            return;
        }

        if (!autoTurnaround)
        {
            autoTurnaround = FighterRef.GetComponent<AutoTurnaround>();
        }

        bool facingOpponent = autoTurnaround.IsFacingOpponent(OtherFighterRef);
        if (!facingOpponent)
        {
            Cancel();
            return;
        }
        
        float distanceToOpponent = Vector3.Distance(FighterRef.transform.position, OtherFighterRef.transform.position);
        Debug.Log(distanceToOpponent);

        if (distanceToOpponent <= spamDistance)
        {
            if (Time.time - lastLaunchTime > .01)
            {
                Cancel();
            }
            if (Time.time - lastLaunchTime > .05)
            {
                Launch();
            }
        }
        else if (distanceToOpponent <= neutralDistance)
        {
            if (Time.time - lastLaunchTime > randomCancelTimeDiff)
            {
                Cancel();
            }
            if (Time.time - lastLaunchTime > randomlaunchTimeDiff)
            {
                Launch();
                randomlaunchTimeDiff = Random.Range(.2f, 1f);
                randomCancelTimeDiff = Random.Range(.5f, 1.5f);
            }
        }
        else
        {
            if (Time.time - lastLaunchTime > randomCancelTimeDiff)
            {
                Cancel();
            }
            if (Time.time - lastLaunchTime > randomlaunchTimeDiff)
            {
                Launch();
                randomlaunchTimeDiff = Random.Range(1f, 1.5f);
                randomCancelTimeDiff = Random.Range(1.5f, 3f);
            }
        }
    }

    public override void Reset()
    {
        base.Reset();
        lastLaunchTime = 0;
        lastCancelTime = 0;
        randomlaunchTimeDiff = 0;
        randomCancelTimeDiff = 0;
    }

    private void Launch()
    {
        FighterRef.StartAction();
        lastLaunchTime = Time.time;
    }

    private void Cancel()
    {
        FighterRef.CancelAction();
        lastCancelTime = Time.time;
    }
}