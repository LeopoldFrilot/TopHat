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
    [SerializeField] private Transform hatSpawnLocation;
    [SerializeField] private GameObject attackerHatPrefab;
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private PlayableDirector countdownTimeline;
    [SerializeField] private Transform leftGameBoundaries;
    [SerializeField] private Transform rightGameBoundaries;
    [SerializeField] private FightSceneUI fightSceneUI;
    
    private PlayerHat defenderHat;
    private List<NetworkedFighterController> networkControllers = new();
    private List<Fighter> players = new();
    private bool vsAI;
    private AIPickerUI pickerUI;
    private bool inCountdown;
    private PlayerInputManager playerInput;
    private int aiAttackModule;
    private int aiDefenseModule;
    private int aiMovementModule;

    public void Awake()
    {
        pickerUI = FindFirstObjectByType<AIPickerUI>();
        playerInput = GetComponent<PlayerInputManager>();
        playerInput.onPlayerJoined += OnPlayerJoined;
        vsAI = PlayerPrefs.GetInt("vsAI") == 1;
        aiAttackModule = PlayerPrefs.GetInt("AIAttackIndex");
        aiDefenseModule = PlayerPrefs.GetInt("AIDefenseIndex");
        aiMovementModule = PlayerPrefs.GetInt("AIMovementIndex");
    }

    private void Update()
    {
        if (players.Count == 2)
        {
            float p1 = players[0].GetHatTime();
            float p2 = players[1].GetHatTime();
            fightSceneUI.UpdatePointsText(p1, p2, Help.Tunables.timeToChargeHat);

            if (p1 >= Help.Tunables.timeToChargeHat || p2 >= Help.Tunables.timeToChargeHat)
            {
                Restart();
            }
        }
    }

    public void ForceAddSecondPlayer()
    {
        if (players.Count != 1)
        {
            return;
        }

        var controller = networkControllers[0];
        
        Fighter fighterRef = controller.SpawnFighter(players.Count == 0 ? player1Prefab : player2Prefab, Vector3.zero, Quaternion.identity);
        fighterRef.OnKnockOff += OnKnockOff;
        players.Add(fighterRef);
        fighterRef.SetPlayerIndex(players.IndexOf(fighterRef) + 1);
        if (players.Count == 2)
        {
            InstallAIModules(aiAttackModule, aiDefenseModule, aiMovementModule);
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
        PlayerPrefs.SetInt("vsAI",  vsAI ? 1 : 0);
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
        if (players.Count == 2)
        {
            return;
        }
        
        networkControllers.Add(networkedFighterController);
        Fighter fighterRef = networkedFighterController.SpawnFighter(players.Count == 0 ? player1Prefab : player2Prefab, Vector3.zero, Quaternion.identity);
        fighterRef.OnKnockOff += OnKnockOff;
        players.Add(fighterRef);
        fighterRef.SetPlayerIndex(players.IndexOf(fighterRef) + 1);
        if (players.Count == 2)
        {
            InstallAIModules(aiAttackModule, aiDefenseModule, aiMovementModule);
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
        SwapRoles();
    }

    private void EnableAIForPlayer(Fighter fighterRef)
    {
        fighterRef.SetAIControlled(true);
    }

    private void StartFight()
    {
        inCountdown = true;
        
        fightSceneUI.SetPlayer1(players[0]);
        fightSceneUI.SetPlayer2(players[1]);
        players[0].transform.position = player1Location.position;
        players[0].SwitchTurnState(TurnState.Attacking);
        players[1].transform.position = player2Location.position;
        players[1].SwitchTurnState(TurnState.Attacking);
        players[1].FaceLeft();
        if (!defenderHat)
        {
            defenderHat = Instantiate(attackerHatPrefab).GetComponent<PlayerHat>();
            defenderHat.OnHatCollected += CollectHat;
        }
        defenderHat.PausePhysics();
        defenderHat.SetNewTarget(null);
        defenderHat.transform.position = hatSpawnLocation.transform.position;
        countdownTimeline.Play();
    }

    public void OnCountdownFinished()
    {
        EventHub.TriggerGameStarted();
        inCountdown = false;
        defenderHat.ResumePhysics();
        countdownTimeline.Stop();
    }

    private void CollectHat(Fighter fighter)
    {
        fighter.SwitchTurnState(TurnState.Defending);
        defenderHat.SetNewTarget(fighter.GetHatLocation());
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

    public int GetAIIndex(AIModuleTypes aiModuleType)
    {
        switch (aiModuleType)
        {
           case AIModuleTypes.Attack:
               return aiAttackModule;
           case AIModuleTypes.Defense:
               return aiDefenseModule;
           case AIModuleTypes.Movement:
               return aiMovementModule;
        }
        
        return 0;
    }

    public bool IsAIEnabled()
    {
        return vsAI;
    }

    public void InstallAIModules(int inAttackModule, int inDefenseModule, int inMovementModule)
    {
        aiAttackModule = inAttackModule;
        aiDefenseModule = inDefenseModule;
        aiMovementModule = inMovementModule;
        PlayerPrefs.SetInt("AIAttackIndex", inAttackModule);
        PlayerPrefs.SetInt("AIDefenseIndex", inDefenseModule);
        PlayerPrefs.SetInt("AIMovementIndex", inMovementModule);
        if (players.Count < 2)
        {
            return;
        }
        
        players[1].InstallAIModules(
            GameWizard.Instance.GetAIModule(AIModuleTypes.Attack, aiAttackModule), 
            GameWizard.Instance.GetAIModule(AIModuleTypes.Defense, aiDefenseModule), 
            GameWizard.Instance.GetAIModule(AIModuleTypes.Movement, aiMovementModule));
    }

    public bool IsInCountdown()
    {
        return inCountdown;
    }

    private void SwapRoles()
    {
        if (players.Count > 1)
        {
            Fighter originalAttacker = players[0].GetTurnState() == TurnState.Attacking ? players[0] : players[1];
            Fighter originalDefender = GetOpponent(originalAttacker);
            originalAttacker.SwapTurnState();
            originalDefender.SwapTurnState();
            CollectHat(originalAttacker);
        }
    }

    private void OnTurnEnded(Fighter fighter)
    {
        SwapRoles();
    }

    public Vector3 ClampToGameplayBounds(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(
                position.x, leftGameBoundaries.position.x, rightGameBoundaries.position.x), 
                position.y,
                position.z);
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