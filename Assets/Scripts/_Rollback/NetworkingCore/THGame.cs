using System;
using System.Collections.Generic;
using Mathematics.Fixed;
using SharedGame;
using Unity.Collections;

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
    public FRect gameBounds;
    public THPlayerRules player1Rules;
    public THPlayerRules player2Rules;
    public FP spawnDistance;
}

[Serializable]
public struct THPlayerRules
{
    public FP bodyCollisionWidth;
    public FP bodyCollisionWHeight;
    public FVector2 bodyCollisionOffset;
}

public class THGame : IGame
{
    private THGameRules gameRules;
    
    private List<THPlayer> players = new();
    private THPlayer player1;
    private THPlayer player2;
    
    public THGame(THGameRules rules)
    {
        gameRules = rules;
        Framenumber = 0;
        
        FP player1X = rules.gameBounds.center.X - rules.spawnDistance;
        player1 = new THPlayer(new(player1X, rules.gameBounds.MinY), rules.player1Rules);
        players.Add(player1);
        
        FP player2X = rules.gameBounds.center.X + rules.spawnDistance;
        player2 = new THPlayer(new(player2X, rules.gameBounds.MinY), rules.player2Rules);
        players.Add(player2);
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
    }

    public void FromBytes(NativeArray<byte> data)
    {
        throw new System.NotImplementedException();
    }

    public NativeArray<byte> ToBytes()
    {
        throw new System.NotImplementedException();
    }

    public long ReadInputs(int controllerId)
    {
        throw new System.NotImplementedException();
    }

    public void LogInfo(string filename)
    {
        throw new System.NotImplementedException();
    }

    public void FreeBytes(NativeArray<byte> data)
    {
        throw new System.NotImplementedException();
    }
}