using System;

public class AIMovementFollow : AIMovementModule
{
    private void Update()
    {
        if (!IsActive())
        {
            return;
        }

        movementRef.RegisterHorizontalInput(GetMovementValueForForward());
    }
}