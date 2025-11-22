using System;
using UnityEngine;

public class AIMovementSmart : AIMovementModule
{
    [SerializeField] private float tooCloseValue = 2f;
    private void Update()
    {
        if (!IsActive())
        {
            return;
        }

        float distanceToOpponent = Vector3.Distance(FighterRef.transform.position, OtherFighterRef.transform.position);
        if (FighterRef.GetTurnState() == TurnState.Attacking && distanceToOpponent > tooCloseValue)
        {
            movementRef.RegisterHorizontalInput(GetMovementValueForForward());
        }
        else
        {
            movementRef.RegisterHorizontalInput(GetMovementValueForBackward());
        }
        
    }
}