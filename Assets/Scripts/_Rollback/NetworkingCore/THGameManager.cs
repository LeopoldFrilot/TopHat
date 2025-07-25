﻿using System.Collections.Generic;
using SharedGame;
using UnityEngine;
using UnityGGPO;

public class THGameManager : GameManager
{
    public THGameRules gameRules;
    public override void StartLocalGame()
    {
        StartGame(new LocalRunner(new THGame(gameRules)));
    }

    public override void StartGGPOGame(IPerfUpdate perfPanel, IList<Connections> connections, int playerIndex)
    {
    }
}