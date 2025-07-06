using UnityEngine;

public class AIPickerUI : MonoBehaviour
{
    [SerializeField] private AIModuleSelector attackSelector;
    [SerializeField] private AIModuleSelector defenderSelector;
    [SerializeField] private AIModuleSelector movementSelector;

    private FightScene fightScene;

    private void Awake()
    {
        fightScene = FindFirstObjectByType<FightScene>();
    }

    public void LockInAIChoice()
    {
        fightScene.InstallAIModules(
            attackSelector.GetModule(),
            defenderSelector.GetModule(),
            movementSelector.GetModule());
    }

    public void TriggerModuleInstallation()
    {
        LockInAIChoice();
    }
}