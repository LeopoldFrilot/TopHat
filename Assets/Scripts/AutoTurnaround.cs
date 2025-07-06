using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoTurnaround : MonoBehaviour
{
    private FightScene fightScene;
    private Player playerRef;

    private void Awake()
    {
        fightScene = FindFirstObjectByType<FightScene>();
        playerRef = GetComponent<Player>();
    }

    private void Update()
    {
        var otherPlayer = fightScene.GetOpponent(playerRef);
        if (otherPlayer != null)
        {
            if (transform.position.x < otherPlayer.transform.position.x)
            {
                playerRef.FaceRight();
            }
            else
            {
                playerRef.FaceLeft();
            }
        }
    }
}