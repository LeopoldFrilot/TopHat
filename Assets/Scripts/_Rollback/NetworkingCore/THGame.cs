using System;
using System.Collections.Generic;
using System.IO;
using Mathematics.Fixed;
using SharedGame;
using Unity.Collections;
using UnityEngine.Serialization;

public static class THConstants {
    public const int MAX_PLAYERS = 2;

    public const int INPUT_LEFT = (1 << 0);
    public const int INPUT_RIGHT = (1 << 1);
    public const int INPUT_UP = (1 << 2);
    public const int INPUT_DOWN = (1 << 3);
    public const int INPUT_ACTION = (1 << 4);
}

[Serializable]
public struct THGameRules
{
    public FP gameBoundsWidth;
    public FP gameBoundsHeight;
    public FVector2 gameBoundsCenter;
    public THPlayerRules player1Rules;
    public THPlayerRules player2Rules;
    public FP spawnDistance;
    public FP gravityForce;
    public FP frictionForce;
    public FP velocityTooLowThreshold;
}

[Serializable]
public struct THPlayerRules
{
    public FP bodyCollisionWidth;
    public FP bodyCollisionHeight;
    public FVector2 bodyCollisionOffset;
    public FVector2 groundedVelocityClamp;
    public FP groundSpeed;
}

public class THGame : IGame
{
    private THGameRules gameRules;
    
    private List<THPlayer> players = new();
    private THPlayer player1;
    private THPlayer player2;
    private THPhysicsManager physics;
    public FRect gameBounds;
    
    public THGame(THGameRules rules)
    {
        gameRules = rules;
        Framenumber = 0;
        physics = new THPhysicsManager();
        gameBounds = new FRect(rules.gameBoundsHeight, rules.gameBoundsWidth, rules.gameBoundsCenter);
        
        FP player1X = gameBounds.center.X - rules.spawnDistance;
        player1 = new THPlayer(new(player1X, gameBounds.MinY), rules.player1Rules);
        players.Add(player1);
        physics.RegisterPhysicsObject(player1);
        
        FP player2X = gameBounds.center.X + rules.spawnDistance;
        player2 = new THPlayer(new(player2X, gameBounds.MinY), rules.player2Rules);
        players.Add(player2);
        physics.RegisterPhysicsObject(player2);
    }
    
    public int Framenumber { get; private set; }
    
    public int Checksum => GetHashCode();
    
    public void Update(long[] inputs, int disconnectFlags)
    {
        Framenumber++;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].inputmanager.ParseInputs(inputs[i]);
        }
        
        for (int i = 0; i < players.Count; i++)
        {
            players[i].movementmanager.Update();
        }
        
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ApplyAllForces(1, players[i].movementmanager.GetCurrentSpeedClamp());
        }
        
        physics.ResolveCollisions(gameBounds);
    }

    public void FromBytes(NativeArray<byte> bytes)
    {
    }

    public NativeArray<byte> ToBytes()
    {
        return new();
    }

    public long ReadInputs(int id)
    {
        long input = 0;
        if (id == 0)
        {
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.W))
            {
                input |= THConstants.INPUT_UP;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.S))
            {
                input |= THConstants.INPUT_DOWN;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.A))
            {
                input |= THConstants.INPUT_LEFT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D))
            {
                input |= THConstants.INPUT_RIGHT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Space))
            {
                input |= THConstants.INPUT_ACTION;
            }
        }
        else if (id == 1)
        {
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow)) 
            {
                input |= THConstants.INPUT_UP;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow)) 
            {
                input |= THConstants.INPUT_DOWN;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftArrow)) 
            {
                input |= THConstants.INPUT_LEFT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightArrow)) 
            {
                input |= THConstants.INPUT_RIGHT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Period))
            {
                input |= THConstants.INPUT_ACTION;
            }
        }
        return input;
    }

    public void LogInfo(string filename)
    {
    }

    public void FreeBytes(NativeArray<byte> data)
    {
    }

    public List<THPlayer> GetPlayers()
    {
        return players;
    }
}