using System;
using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
    private bool blocking = false;
    private float blockingCooldownStart;
    private float blockingTimeStart;
    private Fighter _fighterRef;

    private void Awake()
    {
        _fighterRef = gameObject.GetComponent<Fighter>();
    }

    private void Update()
    {
        if (blocking)
        {
            if (Help.Tunables.blockingTime >= 0)
            {
                if (Time.time >= (Help.Tunables.blockingTime + blockingTimeStart))
                {
                    StopBlocking();
                }
            }
        }
    }

    public void CancelBlockLag()
    {
        StopBlocking(true);
    }

    public void TryToBLock()
    {
        if (CanBlock())
        {
            blocking = true;
            blockingTimeStart = Time.time;
            foreach (var fists in _fighterRef.GetSpawnedFists())
            {
                fists.StartBlock();
            }
        }
    }
    
    public bool IsBlocking() => blocking;

    private void StopBlocking(bool cancelLag = false)
    {
        blocking = false;
        if (!cancelLag)
        {
            blockingCooldownStart = Time.time;
        }
        
        foreach (var fists in _fighterRef.GetSpawnedFists())
        {
            fists.StopBlock(cancelLag);
        }
    }

    private bool CanBlock()
    {
        return !blocking && Time.time >= (blockingCooldownStart + Help.Tunables.blockingCooldown);
    }

    public bool IsPerfectBlock()
    {
        return blocking && blockingTimeStart + Help.Tunables.perfectBlockTime >= Time.time;
    }

    public void HandleActionButtonCancelled()
    {
        if (Help.Tunables.blockingTime < 0)
        {
            StopBlocking(true);
        }
    }
}