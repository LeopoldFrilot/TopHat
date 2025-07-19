public class AIDefenseModule : AIBaseModule
{
    public override AIModuleTypes GetAIModuleType()
    {
        return AIModuleTypes.Defense;
    }

    protected override bool IsActive()
    {
        return base.IsActive() && FighterRef.GetTurnState() == TurnState.Defending;
    }
}