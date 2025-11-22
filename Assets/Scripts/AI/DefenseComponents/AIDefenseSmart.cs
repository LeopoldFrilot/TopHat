using System.Collections.Generic;
using UnityEngine;

public class AIDefenseSmart : AIDefenseModule
{
    [SerializeField] private float spamDistance = 5f;
    [SerializeField] private float neutralDistance = 12f;

    private float lastBlockTime = 0;
    private bool useExpensiveAbility;
    private void Update()
    {
        if (!IsActive())
        {
            return;
        }
        
        float distanceToOpponent = Vector3.Distance(FighterRef.transform.position, OtherFighterRef.transform.position);
        if (distanceToOpponent <= spamDistance)
        {
            if (Time.time - lastBlockTime > .1)
            {
                Block();
            }
            
            if (!useExpensiveAbility && FighterRef.GetMeter() >= Help.Tunables.meterRequirementDashCancel)
            {
                FighterRef.StartDownAction();
                useExpensiveAbility = true;
            }
        }
        else if (distanceToOpponent <= neutralDistance)
        {
            if (OtherFighterRef.IsAFistsOfState(new() { PlayerFistState.Windup, PlayerFistState.Launch }))
            {
                Block();
            }
            
            if (!useExpensiveAbility && FighterRef.GetMeter() >= Help.Tunables.meterRequirementDashCancel)
            {
                FighterRef.StartDownAction();
                useExpensiveAbility = true;
            }
            else if (useExpensiveAbility && FighterRef.GetMeter() >= Help.Tunables.meterRequirementGrapple)
            {
                FighterRef.StartUpAction();
                FighterRef.CancelUpAction();
                useExpensiveAbility = false;
            }
        }
        else
        {
            if (OtherFighterRef.IsAFistsOfState(new() { PlayerFistState.Launch }))
            {
                Block();
            }
            
            if (useExpensiveAbility && FighterRef.GetMeter() >= Help.Tunables.meterRequirementGrapple)
            {
                FighterRef.StartUpAction();
                FighterRef.CancelUpAction();
                useExpensiveAbility = false;
            }
        }
    }

    private void Block()
    {
        FighterRef.StartAction();
        FighterRef.CancelAction();
        lastBlockTime = Time.time;
    }

    public override void Reset()
    {
        base.Reset();
        lastBlockTime = 0;
    }
}