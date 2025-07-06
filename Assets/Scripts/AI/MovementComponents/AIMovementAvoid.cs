using System;

public class AIMovementAvoid : AIMovementModule
{
    private void Update()
    {
        if (!IsActive())
        {
            return;
        }

        movementRef.RegisterHorizontalInput(GetMovementValueForBackward());
    }
}