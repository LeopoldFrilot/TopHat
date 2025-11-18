using System;
using UnityEngine;

public class AIAttackSpam : AIAttackModule
{
    [SerializeField] private float windupTime = 1f;

    private double lastLaunchTime = 0;
    private void Update()
    {
        if (!IsActive())
        {
            return;
        }

        if (lastLaunchTime == 0)
        {
            FighterRef.StartAction();
            lastLaunchTime = Time.time;
        }
        else if (lastLaunchTime + windupTime <= Time.time)
        {
            FighterRef.CancelAction();
            lastLaunchTime = 0;
        }
    }

    public override void Reset()
    {
        base.Reset();
        lastLaunchTime = 0;
    }
}