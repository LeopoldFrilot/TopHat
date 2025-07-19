﻿using System;
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
    protected Fighter FighterRef;
    protected Fighter OtherFighterRef;
    protected AIBaseComponent aiRef;
    protected FightScene fightSceneRef;

    private void Awake()
    {
        fightSceneRef = FindFirstObjectByType<FightScene>();
    }

    public void Initialize(Fighter fighter)
    {
        FighterRef = fighter;
        OtherFighterRef = fightSceneRef.GetOpponent(FighterRef);
        movementRef = FighterRef.GetComponent<PlayerMovement>();
        aiRef = FighterRef.GetComponent<AIBaseComponent>();
    }

    abstract public AIModuleTypes GetAIModuleType();

    private void OnGameStarted()
    {
        OtherFighterRef = fightSceneRef.GetOpponent(FighterRef);
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
        return OtherFighterRef != null && aiRef.IsActive();
    }

    virtual public void Reset()
    {
        
    }
}