using System;
using UnityEngine;
using UnityEngine.UI;

public class AIPickerUI : MonoBehaviour
{
    [SerializeField] private AIModuleSelector attackSelector;
    [SerializeField] private AIModuleSelector defenderSelector;
    [SerializeField] private AIModuleSelector movementSelector;
    [SerializeField] private Toggle enableAIToggle;

    private FightScene fightScene;

    private void Awake()
    {
        fightScene = FindFirstObjectByType<FightScene>();
    }

    private void Start()
    {
        enableAIToggle.SetIsOnWithoutNotify(fightScene.IsAIEnabled());
    }

    public void LockInAIChoice()
    {
        fightScene.InstallAIModules(
            attackSelector.GetModuleIndex(),
            defenderSelector.GetModuleIndex(),
            movementSelector.GetModuleIndex());
    }

    public void TriggerModuleInstallation()
    {
        LockInAIChoice();
    }
}