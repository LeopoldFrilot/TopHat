using System;
using UnityEngine;

public class PlayerGrapple: MonoBehaviour
{
    [SerializeField] private int blocksForGrapple = 3;
    
    int accumulatedBlocks = 0;
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
            SetBlockCount(accumulatedBlocks - blocksForGrapple);
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
        return !inGrapple && accumulatedBlocks >= blocksForGrapple;
    }

    public Action<int, int> OnBlockCountChanged;
    public void SetBlockCount(int newCount)
    {
        if (accumulatedBlocks != newCount)
        {
            accumulatedBlocks = newCount;
            OnBlockCountChanged?.Invoke(accumulatedBlocks, blocksForGrapple);
        }
    }

    public void AddSuccessfulBlock(int quantity = 1)
    {
        SetBlockCount(accumulatedBlocks + quantity);
    }

    public void ResetGrapple()
    {
        StopGrapple();
        SetBlockCount(0);
    }
}