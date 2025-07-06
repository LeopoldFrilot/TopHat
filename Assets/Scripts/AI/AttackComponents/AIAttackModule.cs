public class AIAttackModule : AIBaseModule
{
    public override AIModuleTypes GetAIModuleType()
    {
        return AIModuleTypes.Attack;
    }
    
    protected override bool IsActive()
    {
        return base.IsActive() && playerRef.GetTurnState() == TurnState.Attacking;
    }
}