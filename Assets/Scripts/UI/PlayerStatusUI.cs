using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private Fighter fighter;
    [SerializeField] private Slider statureSlider;
    [SerializeField] private TextMeshProUGUI statureText;
    [SerializeField] private GameObject UIRoot;

    private PlayerDizziness dizzinessRef;
    private PlayerStatus statusRef;

    private void Awake()
    {
        dizzinessRef = fighter.GetComponent<PlayerDizziness>();
        statusRef = fighter.GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        bool somethingShowing = false;
        
        float dizzinessNormalized = dizzinessRef.GetNormalizedDizziness();
        statureSlider.value = dizzinessNormalized;
        
        if (dizzinessNormalized > 0)
        {
            somethingShowing = true;
        }

        List<StatusType> status = statusRef.GetActiveStatusEffects();
        if (status.Count > 0)
        {
            if (status.Contains(StatusType.Stunned))
            {
                statureText.text = $"Stunned!";
                somethingShowing = true;
            }
            else
            {
                statureText.text = "";
            }
        }
        else
        {
            statureText.text = "";
        }

        UIRoot.SetActive(somethingShowing);
    }
}