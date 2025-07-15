using Mathematics.Fixed;
using UnityEngine;

public class THPlayer : THPhysicsObject
{
    // Main components
    public THInputManager inputmanager;
    public THMovementManager movementmanager;
    
    public THPlayerRules playerRules;
    private FRect mainBodyCollider;

    public THPlayer(FVector2 spawnPosition, THPlayerRules inPlayerRules) : base(spawnPosition)
    {
        playerRules = inPlayerRules;
        
        inputmanager = new();
        movementmanager = new(this);
        
        mainBodyCollider = new FRect(
            playerRules.bodyCollisionHeight, 
            playerRules.bodyCollisionWidth, 
            spawnPosition + playerRules.bodyCollisionOffset);
        AttachShape(mainBodyCollider);
    }
}