using System;
using UnityEngine;

public class AIMovementRandom : AIMovementModule
{
    private enum MoveDirection
    {
        Forward,
        Backward,
        Neutral,
        
        
        
        
        Count,
    }

    [SerializeField] private float minRandomTime = .5f;
    [SerializeField] private float maxRandomTime = 3f;
    [SerializeField] private float jumpTryRate = .5f;
    [SerializeField] private float jumpChance = .3f;

    private float lastJumpTry;
    private float nextDirectionChange = 0;
    private MoveDirection currentDirection = MoveDirection.Neutral;

    private void Update()
    {
        if (!IsActive())
        {
            return;
        }

        if (Time.time > nextDirectionChange)
        {
            ChangeRandomDirection();
        }
    }

    private void ChangeRandomDirection()
    {
        currentDirection = (MoveDirection)UnityEngine.Random.Range(0, (int)MoveDirection.Count);
        switch (currentDirection)
        {
            case MoveDirection.Forward:
                movementRef.RegisterHorizontalInput(GetMovementValueForForward());
                break;
            case MoveDirection.Backward:
                movementRef.RegisterHorizontalInput(GetMovementValueForBackward());
                break;
            case MoveDirection.Neutral:
                movementRef.RegisterHorizontalInput(0);
                break;
        }
        nextDirectionChange = Time.time + UnityEngine.Random.Range(minRandomTime, maxRandomTime);
    }
}