using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

public class FightScene : MonoBehaviour
{
    [SerializeField] private Transform player1Location;
    [SerializeField] private Transform player2Location;
    [SerializeField] private GameObject attackerHatPrefab;
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private PlayableDirector countdownTimeline;
    [SerializeField] private float targetHatTime = 60f;
    
    private PlayerHat defenderHat;
    private List<Fighter> players = new();
    private FightSceneUI fightSceneUI;
    private bool vsAI;
    private AIPickerUI pickerUI;
    private bool inCountdown;
    private PlayerInputManager playerInput;

    public void Awake()
    {
        fightSceneUI = GetComponent<FightSceneUI>();
        pickerUI = FindFirstObjectByType<AIPickerUI>();
        playerInput = GetComponent<PlayerInputManager>();
        playerInput.onPlayerJoined += OnPlayerJoined;
    }

    private void Update()
    {
        if (players.Count == 2)
        {
            float p1 = players[0].GetHatTime();
            float p2 = players[1].GetHatTime();
            fightSceneUI.UpdatePointsText(p1, p2, targetHatTime);

            if (p1 >= targetHatTime || p2 >= targetHatTime)
            {
                Restart();
            }
        }
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.GetComponent<NetworkedFighterController>().OnPlayerJoined(playerInput);
    }

    public void Restart()
    {
        players[0].Restart();
        players[1].Restart();
        StartFight();
    }

    public void ToggleAI()
    {
        vsAI = !vsAI;
        if (players.Count == 2)
        {
            if (vsAI)
            {
                EnableAIForPlayer(players[1]);
            }
            else
            {
                players[1].SetAIControlled(false);
            }
        }
    }

    public void RegisterFighter(NetworkedFighterController networkedFighterController)
    {
        Fighter fighterRef = networkedFighterController.SpawnFighter(players.Count == 0 ? player1Prefab : player2Prefab, Vector3.zero, Quaternion.identity);
        fighterRef.OnKnockOff += OnKnockOff;
        players.Add(fighterRef);
        fighterRef.SetPlayerIndex(players.IndexOf(fighterRef) + 1);
        if (players.Count == 2)
        {
            if (vsAI)
            {
                EnableAIForPlayer(players[1]);
            }
            else
            {
                players[1].SetAIControlled(false);
            }
            
            Restart();
        }
    }

    private void OnKnockOff()
    {
        StartCoroutine(StartSwapRoles());
    }

    private void EnableAIForPlayer(Fighter fighterRef)
    {
        pickerUI.TriggerModuleInstallation();
        fighterRef.SetAIControlled(true);
    }

    private void StartFight()
    {
        inCountdown = true;
        int randomFirst = Random.Range(0, 2);
        players[0].transform.position = player1Location.position;
        players[0].SwitchTurnState(randomFirst == 0 ? TurnState.Attacking : TurnState.Defending);
        players[1].transform.position = player2Location.position;
        players[1].SwitchTurnState(randomFirst == 1 ? TurnState.Attacking : TurnState.Defending);
        players[1].FaceLeft();
        if (!defenderHat)
        {
            defenderHat = Instantiate(attackerHatPrefab).GetComponent<PlayerHat>();
        }
        StartCoroutine(StartSwapRoles());
        countdownTimeline.Play();
    }

    public void OnCountdownFinished()
    {
        EventHub.TriggerGameStarted();
        inCountdown = false;
    }

    public Fighter GetOpponent(Fighter fighter)
    {
        foreach (var player1 in players)
        {
            if (player1 != fighter)
            {
                return player1;
            }
        }
        
        return null;
    }

    public void InstallAIModules(GameObject attackModule, GameObject defenseModule, GameObject movementModule)
    {
        if (players.Count < 2)
        {
            return;
        }
        
        players[1].InstallAIModules(attackModule, defenseModule, movementModule);
    }

    public bool IsInCountdown()
    {
        return inCountdown;
    }

    private IEnumerator StartSwapRoles()
    {
        if (players.Count > 1)
        {
            Fighter originalAttacker = players[0].GetTurnState() == TurnState.Attacking ? players[0] : players[1];
            Fighter originalDefender = GetOpponent(originalAttacker);
            originalAttacker.SwitchTurnState(TurnState.Defending);
            defenderHat.SetNewTarget(originalAttacker.GetHatLocation());
            yield return new WaitUntil(()=>defenderHat.IsInTransition() == false);
            originalDefender.SwapTurnState();
        }
    }

    private void OnTurnEnded(Fighter fighter)
    {
        StartCoroutine(StartSwapRoles());
    }

    private void OnEnable()
    {
        EventHub.OnTurnEnded += OnTurnEnded;
    }

    private void OnDisable()
    {
        EventHub.OnTurnEnded -= OnTurnEnded;
    }
}