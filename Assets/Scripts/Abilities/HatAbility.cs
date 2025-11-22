using System;
using UnityEngine;

public abstract class HatAbility : MonoBehaviour
{
    [SerializeField] private float meterCost;
    [SerializeField] private AbilityTypes abilityType;
    
    protected Fighter fighterRef;
    
    protected virtual void Awake()
    {
        fighterRef = transform.root.GetComponentInChildren<Fighter>();
    }

    public float GetMeterCost()
    {
        return meterCost;
    }

    public bool HasMeterForAbility()
    {
        if (!fighterRef)
        {
            return false;
        }
        
        return fighterRef.GetMeter() >= meterCost;
    }

    public virtual bool CanActivate()
    {
        return HasMeterForAbility();
    }

    public void TryActivate()
    {
        if (CanActivate())
        {
            fighterRef.ChangeMeter(-meterCost);
            Activate();
        }
    }
    public void TryCancel()
    {
        Cancel();
    }

    public virtual void Activate()
    {
    }

    public virtual void Cancel()
    {
    }

    public AbilityTypes GetAbilityType()
    {
        return abilityType;
    }
}