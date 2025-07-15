using System;
using System.Collections.Generic;
using Mathematics.Fixed;
using SharedGame;
using UnityEditor;
using UnityEngine;

public class THGameViewModel : MonoBehaviour, IGameView
{
    [SerializeField] private TopHatPlayer topHatPrefab;

    private List<TopHatPlayer> spawnedPlayers = null;
    private GameManager gameManager => GameManager.Instance;

    private void Update()
    {
        if (gameManager && gameManager.IsRunning)
        {
            UpdateGameView(gameManager.Runner);
        }
    }
    
    public void UpdateGameView(IGameRunner runner)
    {
        THGame game = (THGame)runner.Game;
        
        List<THPlayer> players = game.GetPlayers();
        if (spawnedPlayers == null)
        {
            spawnedPlayers = new();
            foreach (var player in players)
            {
                Vector3 location = new Vector3(player.transform.position.X.ToFloat(),
                    player.transform.position.Y.ToFloat(), 0);
                TopHatPlayer newPlayer = Instantiate(topHatPrefab, location, Quaternion.identity);
                spawnedPlayers.Add(newPlayer);
            }
        }

        for (int i = 0; i < spawnedPlayers.Count; i++)
        {
            spawnedPlayers[i].transform.position = new Vector3(players[i].transform.position.X.ToFloat(), players[i].transform.position.Y.ToFloat(), 0f);
        }
    }

    private void OnDrawGizmos()
    {
        if (gameManager && gameManager.IsRunning)
        {
            THGame game = (THGame)gameManager.Runner.Game;
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(
                THStatics.FVector2ToVector3(game.gameBounds.center, 0), 
                THStatics.FVector2ToVector3(new FVector2(game.gameBounds.width, game.gameBounds.height), 1));
        }
    }
}