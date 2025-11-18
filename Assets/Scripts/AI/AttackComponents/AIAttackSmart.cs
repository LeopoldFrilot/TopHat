using System;
using UnityEngine;

public class AIAttackSmart : AIAttackModule
{
    [SerializeField] private float spamDistance = 1.5f;
    [SerializeField] private float neutralDistance = 3.5f;
    private AutoTurnaround autoTurnaround;
    private double lastLaunchTime = 0;

    protected override void Awake()
    {
        base.Awake();
        autoTurnaround = FighterRef.GetComponent<AutoTurnaround>();
    }
    private void Update()
    {
        if (!IsActive())
        {
            return;
        }

        float distanceToOpponent = Vector3.Distance(FighterRef.transform.position, OtherFighterRef.transform.position);
        bool facingOpponent = autoTurnaround.IsFacingOpponent(OtherFighterRef);

        if (!facingOpponent)
        {
            FighterRef.CancelAction();
            return;
        }

        if (distanceToOpponent <= spamDistance)
        {
            if (Time.time - lastLaunchTime > .05)
            {
                Launch();
            }
        }
        else if (distanceToOpponent <= neutralDistance)
        {
            
        }
    }

    private void Launch()
    {
        FighterRef.StartAction();
        lastLaunchTime = Time.time;
    }
}