using System;
using System.Collections.Generic;
using UnityEngine;

public enum AIModuleTypes
{
    Attack,
    Defense,
    Movement
}
public abstract class AIBaseModule : MonoBehaviour
{
    protected PlayerMovement movementRef;
    protected Player playerRef;
    protected Player otherPlayerRef;
    protected AIBaseComponent aiRef;
    protected FightScene fightSceneRef;

    private void Awake()
    {
        fightSceneRef = FindFirstObjectByType<FightScene>();
    }

    public void Initialize(Player player)
    {
        playerRef = player;
        otherPlayerRef = fightSceneRef.GetOpponent(playerRef);
        movementRef = playerRef.GetComponent<PlayerMovement>();
        aiRef = playerRef.GetComponent<AIBaseComponent>();
    }

    abstract public AIModuleTypes GetAIModuleType();

    private void OnGameStarted()
    {
        otherPlayerRef = fightSceneRef.GetOpponent(playerRef);
    }

    private void OnEnable()
    {
        EventHub.OnGameStarted += OnGameStarted;
    }

    private void OnDisable()
    {
        EventHub.OnGameStarted -= OnGameStarted;
    }

    virtual protected bool IsActive()
    {
        return otherPlayerRef != null && aiRef.IsActive();
    }

    virtual public void Reset()
    {
        
    }
}