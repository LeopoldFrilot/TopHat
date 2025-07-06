public class AIMovementModule : AIBaseModule
{
    public override AIModuleTypes GetAIModuleType()
    {
        return AIModuleTypes.Movement;
    }

    protected int GetMovementValueForForward()
    {
        return otherPlayerRef.transform.position.x < transform.position.x ? -1 : 1;
    }

    protected int GetMovementValueForBackward()
    {
        return otherPlayerRef.transform.position.x < transform.position.x ? 1 : -1;
    }

    public override void Reset()
    {
        base.Reset();
        movementRef.RegisterHorizontalInput(0);
    }
}