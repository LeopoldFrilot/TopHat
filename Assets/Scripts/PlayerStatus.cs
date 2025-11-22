using System;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType
{
    Grappled,
    Stunned,
    AbilityLag,
    Invulnerable,
    HatInactive,
}

public struct StatusEffect
{
    public bool valid;
    public StatusType type;
}

public class PlayerStatus : MonoBehaviour
{
    private List<StatusEffect> statusEffects = new();

    public int AddStatusEffect(StatusType type)
    {
        /*for (int i = 0; i < statusEffects.Count; i++)
        {
            var status = statusEffects[i];
            if (status.valid)
            {
                continue;
            }

            statusEffects[i] = new StatusEffect{type = type, valid = true};
            TriggerStatusEffectsChanged();
            
            return i;
        }*/
        
        statusEffects.Add(new StatusEffect{type = type, valid = true});
        TriggerStatusEffectsChanged();
        return statusEffects.Count - 1;
    }

    public void RemoveStatusEffect(int handle)
    {
        if (handle < statusEffects.Count && handle >= 0)
        {
            statusEffects[handle] = new StatusEffect{valid = false};
            TriggerStatusEffectsChanged();
        }
    }

    public void RemoveStatusEffectsOfType(StatusType type)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].valid && statusEffects[i].type == type)
            {
                RemoveStatusEffect(i);
            }
        }
    }

    public List<StatusType> GetActiveStatusEffects()
    {
        List<StatusType> activeEffects = new();
        foreach (var statusEffect in statusEffects)
        {
            if (statusEffect.valid && !activeEffects.Contains(statusEffect.type))
            {
                activeEffects.Add(statusEffect.type);
            }
        }
        
        return activeEffects;
    }
    
    public Action<List<StatusType>> OnStatusEffectsChanged;
    private void TriggerStatusEffectsChanged()
    {
        OnStatusEffectsChanged?.Invoke(GetActiveStatusEffects());
    }
}