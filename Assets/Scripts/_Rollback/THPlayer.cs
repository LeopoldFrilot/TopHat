using Mathematics.Fixed;
using UnityEngine;

public class THPlayer : THObject
{
    // Main components
    public THInputManager inputmanager;
    public THMovementManager movementmanager;
    
    private FRect mainBodyCollider;

    public THPlayer(FVector2 spawnPosition, THPlayerRules playerRules) : base(spawnPosition)
    {
        inputmanager = new();
        movementmanager = new(this);
        
        mainBodyCollider = new FRect(
            playerRules.bodyCollisionWHeight, 
            playerRules.bodyCollisionWidth, 
            spawnPosition + playerRules.bodyCollisionOffset);
        AttachShape(mainBodyCollider);
    }
}