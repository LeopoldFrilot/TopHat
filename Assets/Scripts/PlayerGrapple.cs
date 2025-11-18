using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapple: MonoBehaviour
{
    [SerializeField] private int meterForGrapple = 3;
    
    private bool inGrapple = false;
    private Fighter _fighterRef;
    private List<PlayerFistState> validFistStatesForGrapple = new() { PlayerFistState.Idle };

    private void Awake()
    {
        _fighterRef = gameObject.GetComponent<Fighter>();
    }

    public bool TryToGrapple()
    {
        if (CanGrapple())
        {
            _fighterRef.SetGrappled();
            inGrapple = true;
            _fighterRef.ChangeMeter(-meterForGrapple);
            _fighterRef.StartGrappleAnimation();
            return true;
        }
        
        return false;
    }
    
    public bool IsGrappling() => inGrapple;

    private void StopGrapple()
    {
        inGrapple = false;
    }

    private bool CanGrapple()
    {
        if (!_fighterRef.AreFistsOfState(validFistStatesForGrapple))
        {
            return false;
        }
        
        return !inGrapple && _fighterRef.GetMeter() >= meterForGrapple;
    }

    public void ResetGrapple()
    {
        StopGrapple();
    }

    public int GetMeterRequirement()
    {
        return meterForGrapple;
    }
}