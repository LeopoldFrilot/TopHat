using System;

public class AIMovementSmart : AIMovementModule
{
    private void Update()
    {
        if (!IsActive())
        {
            return;
        }

        if (FighterRef.GetTurnState() == TurnState.Attacking)
        {
            movementRef.RegisterHorizontalInput(GetMovementValueForForward());
        }
        else
        {
            movementRef.RegisterHorizontalInput(GetMovementValueForBackward());
        }
        
    }
}