using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkedFighterController : NetworkBehaviour
{
    private FightScene fightScene;
    Fighter fighterRef;
    Fighter secondFighterRef;
    private bool isKeyboard = true;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (GameWizard.Instance.IsNetworkedGame())
        {
            return;
        }
        
        isKeyboard = playerInput.devices[0].description.deviceClass.Contains("Key");

        GameWizard.Instance.SetGameAsLocal();
        fightScene = FindAnyObjectByType<FightScene>();
        fightScene.RegisterFighter(this);
    }

    public bool IsKeyboardPrimary()
    {
        return isKeyboard;
    }

    public override void OnNetworkSpawn()
    {
        if (GameWizard.Instance.IsLocalGame())
        {
            return;
        }
        
        GameWizard.Instance.SetGameAsNetworked();
        if (IsServer)
        {
            StartCoroutine(SpawnDelay());
        }
    }

    private IEnumerator SpawnDelay()
    {
        while (!IsSpawned || !NetworkManager.Singleton.IsConnectedClient)
        {
            yield return null;
        }
        
        yield return null;
        
        fightScene = FindAnyObjectByType<FightScene>();
        fightScene.RegisterFighter(this);
    }

    public void NetLog(string message)
    {
        Debug.Log($"{(IsClient ? "Client:" : "Host:")} {message}");
    }

    public Fighter SpawnFighter(GameObject fighterPrefab, Vector3 position, Quaternion rotation)
    {
        Debug.Log("About to spawn Fighter! IsSpawned: " + NetworkManager.Singleton.IsServer + " | Prefab Hash: " + NetworkManager.Singleton.NetworkConfig.Prefabs.Contains(fighterPrefab));
        GameObject fighter = Instantiate(fighterPrefab, position, rotation);
        Debug.Log("Spawn Fighter! IsSpawned: " + NetworkManager.Singleton.IsServer + " | Prefab Hash: " + NetworkManager.Singleton.NetworkConfig.Prefabs.Contains(fighterPrefab));
        if (!fighterRef)
        {
            fighterRef = fighter.GetComponent<Fighter>();
            fighterRef.Initialize(this, fightScene);
            if (GameWizard.Instance.IsNetworkedGame())
            {
                fighterRef.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
            }
            
            return fighterRef;
        }
        else
        {
            secondFighterRef = fighter.GetComponent<Fighter>();
            secondFighterRef.Initialize(this, fightScene);
            if (GameWizard.Instance.IsNetworkedGame())
            {
                secondFighterRef.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
            }
            
            return secondFighterRef;
        }
    }

    public int GetPlayerIndex(Fighter fighter)
    {
        return fighter == fighterRef ? 0 : 1;
    }
}