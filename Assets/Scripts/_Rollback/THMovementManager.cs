using Mathematics.Fixed;
using UnityEngine;

public class THMovementManager
{
    private THInputManager inputManagerRef;
    private THPlayer playerRef;

    private FP movementVector;
    
    public THMovementManager(THPlayer player)
    {
        playerRef = player;
        inputManagerRef = player.inputmanager;
        inputManagerRef.OnInputStarted += OnInputStarted;
        inputManagerRef.OnInputHeld += OnInputHeld;
        inputManagerRef.OnInputEnded += OnInputEnded;
    }

    private void OnInputEnded(THInputs input)
    {
        switch (input)
        {
            case THInputs.Left:
                if (movementVector < 0)
                {
                    movementVector = FP.Zero;
                }
                break;
            case THInputs.Right:
                if (movementVector > 0)
                {
                    movementVector = FP.Zero;
                }
                break;
        }
    }

    private void OnInputHeld(THInputs input)
    {
    }

    private void OnInputStarted(THInputs input)
    {
        switch (input)
        {
            case THInputs.Left:
                movementVector = FP.MinusOne;
                break;
            case THInputs.Right:
                movementVector = FP.One;
                break;
        }
    }

    public void Update()
    {
        playerRef.GenerateAppliedForce(new FVector2(movementVector * playerRef.playerRules.groundSpeed, FP.Zero), AppliedForceTypes.Impulse);
    }

    public FVector2 GetCurrentSpeedClamp()
    {
        // Eventually differentiate between grounded and aerial
        return playerRef.playerRules.groundedVelocityClamp;
    }
}