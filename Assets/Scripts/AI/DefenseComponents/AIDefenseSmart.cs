using System.Collections.Generic;
using UnityEngine;

public class AIDefenseSmart : AIDefenseModule
{
    [SerializeField] private float spamDistance = 5f;
    [SerializeField] private float neutralDistance = 12f;

    private float lastBlockTime = 0;
    private void Update()
    {
        if (!IsActive())
        {
            return;
        }
        
        float distanceToOpponent = Vector3.Distance(FighterRef.transform.position, OtherFighterRef.transform.position);
        Debug.Log(distanceToOpponent);

        if (distanceToOpponent <= spamDistance)
        {
            if (Time.time - lastBlockTime > .01)
            {
                Block();
            }
        }
        else if (distanceToOpponent <= neutralDistance)
        {
            if (OtherFighterRef.IsAFistsOfState(new() { PlayerFistState.Windup, PlayerFistState.Launch }))
            {
                Block();
            }
        }
        else
        {
            if (OtherFighterRef.IsAFistsOfState(new() { PlayerFistState.Launch }))
            {
                Block();
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