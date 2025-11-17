using System;
using UnityEngine;

public class PlayerGrapple: MonoBehaviour
{
    [SerializeField] private int meterForGrapple = 3;
    
    private bool inGrapple = false;
    private Fighter _fighterRef;

    private void Awake()
    {
        _fighterRef = gameObject.GetComponent<Fighter>();
    }

    public void TryToGrapple()
    {
        if (CanGrapple())
        {
            _fighterRef.SetGrappled();
            inGrapple = true;
            _fighterRef.ChangeMeter(meterForGrapple);
            _fighterRef.StartGrappleAnimation();
        }
    }
    
    public bool IsGrappling() => inGrapple;

    private void StopGrapple()
    {
        inGrapple = false;
    }

    private bool CanGrapple()
    {
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