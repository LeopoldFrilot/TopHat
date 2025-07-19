public class AIAttackModule : AIBaseModule
{
    public override AIModuleTypes GetAIModuleType()
    {
        return AIModuleTypes.Attack;
    }
    
    protected override bool IsActive()
    {
        return base.IsActive() && FighterRef.GetTurnState() == TurnState.Attacking;
    }
}