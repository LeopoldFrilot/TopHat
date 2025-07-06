using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class FightScene : MonoBehaviour
{
    [SerializeField] private Transform player1Location;
    [SerializeField] private Transform player2Location;
    [SerializeField] private float grappleResetPower = 5f;
    [SerializeField] private GameObject attackerHatPrefab;
    
    private PlayerHat attackerHat;
    private List<Player> players = new();
    private PlayerInputManager inputManager;
    private bool started = false;
    private FightSceneUI fightSceneUI;
    private bool vsAI;
    private AIPickerUI pickerUI;

    public void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
        fightSceneUI = GetComponent<FightSceneUI>();
        pickerUI = FindFirstObjectByType<AIPickerUI>();
        inputManager.onPlayerJoined += OnPlayerJoined;
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

    private void OnPlayerJoined(PlayerInput player)
    {
        Player playerRef = player.GetComponent<Player>();
        players.Add(playerRef);
        playerRef.SetPlayerIndex(players.IndexOf(playerRef) + 1);
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
            
            StartFight();
        }
    }

    private void EnableAIForPlayer(Player playerRef)
    {
        
        pickerUI.TriggerModuleInstallation();
        playerRef.SetAIControlled(true);
    }

    private void StartFight()
    {
        int randomFirst = Random.Range(0, 2);
        players[0].transform.position = player1Location.position;
        players[0].SwitchTurnState(randomFirst == 0 ? TurnState.Attacking : TurnState.Defending);
        players[1].transform.position = player2Location.position;
        players[1].SwitchTurnState(randomFirst == 1 ? TurnState.Attacking : TurnState.Defending);
        players[1].FaceLeft();
        if (!attackerHat)
        {
            attackerHat = Instantiate(attackerHatPrefab).GetComponent<PlayerHat>();
        }
        StartCoroutine(StartSwapRoles());
        started = true;
        EventHub.TriggerGameStarted();
    }

    public Player GetOpponent(Player player)
    {
        foreach (var player1 in players)
        {
            if (player1 != player)
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

    private IEnumerator StartSwapRoles()
    {
        Player originalAttacker = players[0].GetTurnState() == TurnState.Attacking ? players[0] : players[1];
        Player originalDefender = GetOpponent(originalAttacker);
        originalAttacker.SwitchTurnState(TurnState.Defending);
        attackerHat.SetNewTarget(originalDefender.GetHatLocation());
        yield return new WaitUntil(()=>attackerHat.IsInTransition() == false);
        originalDefender.SwapTurnState();
    }

    private void OnPlayerEarnedPoints(Player obj)
    {
        fightSceneUI.UpdatePointsText(players[0].GetPointsThisGame(), players[1].GetPointsThisGame());
    }

    private void OnPlayerGrappled(Player playerGrappled)
    {
        playerGrappled.GetComponent<PlayerMovement>().LaunchPlayer(new Vector2((GetOpponent(playerGrappled).IsFacingLeft() ? 1f : -1f) * 1.2f, 1f) * grappleResetPower);
        StartCoroutine(StartSwapRoles());
    }

    private void OnTurnEnded(Player player)
    {
        StartCoroutine(StartSwapRoles());
    }

    private void OnEnable()
    {
        EventHub.OnTurnEnded += OnTurnEnded;
        EventHub.OnPlayerEarnedPoints += OnPlayerEarnedPoints;
        EventHub.OnPlayerGrappled += OnPlayerGrappled;
    }

    private void OnDisable()
    {
        EventHub.OnTurnEnded -= OnTurnEnded;
        EventHub.OnPlayerEarnedPoints -= OnPlayerEarnedPoints;
        EventHub.OnPlayerGrappled -= OnPlayerGrappled;
    }
}