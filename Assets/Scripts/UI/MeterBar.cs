using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct AbilityIconMapping
{
    public AbilityTypes abilityType;
    public Sprite icon;
}

public class MeterBar : MonoBehaviour
{
    [SerializeField] private GameObject fillBar;
    [SerializeField] private Slider slider;
    [SerializeField] private List<Transform> abilityLocations = new();
    [SerializeField] private List<AbilityIconMapping> abilityIconMappings = new();
    [SerializeField] private AbilityIcon abilityIconPrefab;

    private Fighter fighterRef;
    private PlayerMeter meter;
    private List<AbilityIcon> spawnedIcons = new();
    
    private void Awake()
    {
        slider.onValueChanged.AddListener(OnValueChanged); 
    }

    public void SetFighter(Fighter fighter)
    {
        if (fighterRef == fighter)
        {
            return;
        }

        if (fighterRef)
        {
            meter.OnMeterChanged -= OnMeterChanged;
            fighterRef.OnTurnStateChanged -= OnTurnStateChanged;
        }
        
        fighterRef = fighter;
        OnMeterChanged(fighterRef.GetMeter());
        OnTurnStateChanged(fighterRef.GetTurnState());
        meter = fighterRef.GetComponent<PlayerMeter>();
        meter.OnMeterChanged += OnMeterChanged;
        fighterRef.OnTurnStateChanged += OnTurnStateChanged;
    }

    private void OnTurnStateChanged(TurnState newState)
    {
        DestroyAbilitySignifiers();
        if (newState == TurnState.Attacking)
        {
            AddAbilitySignifier(Mathf.RoundToInt(Help.Tunables.meterRequirementGrapple),
                GetSpriteFromMapping(AbilityTypes.Grapple));
        }
        else
        {
            AddAbilitySignifier(Mathf.RoundToInt(Help.Tunables.meterRequirementDodgeRoll),
                GetSpriteFromMapping(AbilityTypes.DodgeRoll));
        }
    }

    private Sprite GetSpriteFromMapping(AbilityTypes abilityType)
    {
        foreach (var abilityIconMapping in abilityIconMappings)
        {
            if (abilityIconMapping.abilityType == abilityType)
            {
                return abilityIconMapping.icon;
            }
        }
        
        return null;
    }

    private void DestroyAbilitySignifiers()
    {
        for (int i = spawnedIcons.Count - 1; i >= 0; i--)
        {
            Destroy(spawnedIcons[i].gameObject);
        }
        
        spawnedIcons.Clear();
    }

    private void AddAbilitySignifier(int position, Sprite icon)
    {
        AbilityIcon newIcon = Instantiate(abilityIconPrefab, abilityLocations[position]);
        newIcon.Initialize(icon, position);
        spawnedIcons.Add(newIcon);
    }

    private void OnMeterChanged(float newMeter)
    {
        slider.value = newMeter / Help.Tunables.maxMeter;
        foreach (var icon in spawnedIcons)  
        {
            if (!icon)
            {
                continue;
            }
            
            icon.UpdateMeter(newMeter);
        }
    }

    private void OnValueChanged(float newValue)
    {
        fillBar.SetActive(newValue > 0);
    }
}