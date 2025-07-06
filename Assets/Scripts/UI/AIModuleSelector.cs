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

    private void RefreshModules()
    {
        modulesForThisType.Clear();
        foreach (GameObject module in GameWizard.Instance.allAIModules)
        {
            if (!module)
            {
                continue;
            }

            AIBaseModule moduleCast = module.GetComponent<AIBaseModule>();
            if (!moduleCast)
            {
                return;
            }

            if (moduleCast.GetAIModuleType() == aiModuleType)
            {
                modulesForThisType.Add(module);
            }
        }
    }

    public void GoLeft()
    {
        TrySwitchIndex(index - 1);
    }

    public void GoRight()
    {
        TrySwitchIndex(index + 1);
    }

    private void TrySwitchIndex(int newIndex)
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

    private void OnEnable()
    {
        RefreshModules();
        TrySwitchIndex(index);
    }
}