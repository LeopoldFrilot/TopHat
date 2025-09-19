using System;
using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
    [SerializeField] private float blockingTime = 0.5f;
    [SerializeField] private float perfectBlockTime = .2f;
    [SerializeField] private float blockingCooldown = 0.5f;
    private bool blocking = false;
    private float blockingCooldownStart;
    private float parryTimeStart;
    private Fighter _fighterRef;

    private void Awake()
    {
        _fighterRef = gameObject.GetComponent<Fighter>();
    }

    public void TryToParry()
    {
        parryTimeStart = Time.time;
    }

    public void StartBlocking()
    {
        blocking = true;
        foreach (var fists in _fighterRef.GetSpawnedFists())
        {
            fists.StartBlock();
        }
    }
    
    public bool IsBlocking() => blocking;

    public void StopBlocking(bool cancelLag = false)
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
        return !blocking && Time.time >= (blockingCooldownStart + blockingCooldown);
    }

    public bool IsPerfectBlock()
    {
        return blocking && parryTimeStart + perfectBlockTime >= Time.time;
    }
}