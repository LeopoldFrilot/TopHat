using System;
using UnityEngine;

public class AIAttackSpam : AIAttackModule
{
    [SerializeField] private float windupTime = 1f;

    private float lastLaunchTime = 0;
    private void Update()
    {
        if (!IsActive())
        {
            return;
        }

        if (lastLaunchTime == 0)
        {
            playerRef.StartAction();
            lastLaunchTime = Time.time;
        }
        else if (lastLaunchTime + windupTime <= Time.time)
        {
            playerRef.CancelAction();
            lastLaunchTime = 0;
        }
    }

    public override void Reset()
    {
        base.Reset();
        lastLaunchTime = 0;
    }
}