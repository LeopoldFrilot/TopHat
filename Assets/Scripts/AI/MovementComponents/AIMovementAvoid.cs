using System;

public class AIMovementAvoid : AIMovementModule
{
    private void Update()
    {
        if (!IsActive())
        {
            return;
        }

        movementRef.RegisterHorizontalInput(otherPlayerRef.transform.position.x < transform.position.x ? 1 : -1);
    }
}