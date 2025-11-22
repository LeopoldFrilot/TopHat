using System.Collections.Generic;
using UnityEngine;

public class AutoTurnaround : MonoBehaviour
{
    private FightScene fightScene;
    private Fighter _fighterRef;

    private List<PlayerFistState> validFistStatesForTurnaround = new() { PlayerFistState.Idle, PlayerFistState.Retract, PlayerFistState.Launch};

    private void Awake()
    {
        fightScene = FindFirstObjectByType<FightScene>();
        _fighterRef = GetComponent<Fighter>();
    }

    private void Update()
    {
        var otherPlayer = fightScene.GetOpponent(_fighterRef);
        if (otherPlayer == null)
        {
            return;
        }

        if (!_fighterRef.CanMove())
        {
            return;
        }

        if (!_fighterRef.AreFistsOfState(validFistStatesForTurnaround))
        {
            return;
        }

        if (IsFacingOpponent(otherPlayer))
        {
            return;
        }
        
        if (transform.position.x < otherPlayer.transform.position.x)
        {
            _fighterRef.FaceRight();
        }
        else
        {
            _fighterRef.FaceLeft();
        }
    }

    public bool IsFacingOpponent(Fighter otherPlayer)
    { 
        if (transform.position.x < otherPlayer.transform.position.x)
        {
            return !_fighterRef.IsFacingLeft();
        }
        else
        {
            return _fighterRef.IsFacingLeft();
        }
    }
}