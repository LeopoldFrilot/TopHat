using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AIModuleSelector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moduleName;
    [SerializeField] AIModuleTypes aiModuleType;
    
    private List<GameObject> modulesForThisType = new();
    private int index = 0;
    FightScene fightScene;

    private void Awake()
    {
        fightScene = FindObjectOfType<FightScene>();
    }

    private void RefreshModules()
    {
        modulesForThisType.Clear();
        GameWizard.Instance.allAIModuleMap.TryGetValue(aiModuleType, out modulesForThisType);
        TrySwitchIndex(fightScene.GetAIIndex(aiModuleType));
    }

    public void GoLeft()
    {
        TrySwitchIndex(index - 1);
    }

    public void GoRight()
    {
        TrySwitchIndex(index + 1);
    }

    public void TrySwitchIndex(int newIndex)
    {
        if (newIndex < 0 || newIndex >= modulesForThisType.Count)
        {
            return;
        }
        
        index = newIndex;
        string[] names = modulesForThisType[index].name.Split('_');
        moduleName.text = names[^1];
    }

    public GameObject GetModule()
    {
        RefreshModules();
        TrySwitchIndex(index);
        return modulesForThisType[index];
    }

    public int GetModuleIndex()
    {
        return index;
    }

    private void OnEnable()
    {
        RefreshModules();
    }
}