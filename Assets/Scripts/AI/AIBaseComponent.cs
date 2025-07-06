using System;
using System.Collections.Generic;
using UnityEngine;

public class AIBaseComponent : MonoBehaviour
{
    private AIMovementModule movementModule;
    private AIAttackModule attackModule;
    private AIDefenseModule defenseModule;
    private Player playerRef;
    private bool active = false;
    
    private List<GameObject> installedModules = new();

    private void Awake()
    {
        playerRef = GetComponent<Player>();
        playerRef.OnTurnStateChanged += OnTurnStateChanged;
        movementModule = GetComponent<AIMovementModule>();
        attackModule = GetComponent<AIAttackModule>();
        defenseModule = GetComponent<AIDefenseModule>();
    }

    private void OnTurnStateChanged(TurnState turnState)
    {
        if (turnState == TurnState.Attacking)
        {
            if (attackModule) attackModule.Reset();
        }
        else if (turnState == TurnState.Defending)
        {
            if (defenseModule) defenseModule.Reset();
        }
    }

    public void SetActive(bool value)
    {
        active = value;
    }

    public bool IsActive()
    {
        return active;
    }

    public void InstallAIModules(GameObject attackModule, GameObject defenseModule, GameObject movementModule)
    {
        foreach (var module in installedModules)
        {
            if (!module)
            {
                continue;
            }
            module.GetComponent<AIBaseModule>().Reset();
            Destroy(module);
        }
        
        installedModules.Clear();

        foreach (GameObject module in new List<GameObject>{attackModule, defenseModule, movementModule})
        {
            if (!module)
            {
                continue;
            }
            
            GameObject newModule = Instantiate(module, transform);
            AIBaseModule aiBaseModule = newModule.GetComponent<AIBaseModule>(); 
            aiBaseModule.Initialize(GetComponent<Player>());
        }
    }

    public void Activate()
    {
        throw new NotImplementedException();
    }

    public void Deactivate()
    {
        InstallAIModules(null, null, null);
    }
}