using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameWizard : MonoBehaviour
{
    public List<GameObject> allAIModules = new();

    #region Singleton

    // Static reference to the singleton instance of GameWizard
    private static GameWizard _instance;
    public static GameWizard Instance => _instance;

    private void Awake()
    {
        // Ensure only one instance of GameWizard exists
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            NonSingletonAwake();
            // Don't destroy GameWizard on scene changes
            DontDestroyOnLoad(_instance.gameObject);
        }
    }

    #endregion

    private void NonSingletonAwake()
    {
        
    }
}