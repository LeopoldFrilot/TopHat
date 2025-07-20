using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkedFighterController : NetworkBehaviour
{
    private FightScene fightScene;
    Fighter fighterRef;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (GameWizard.Instance.IsNetworkedGame())
        {
            return;
        }

        GameWizard.Instance.SetGameAsLocal();
        fightScene = FindAnyObjectByType<FightScene>();
        fightScene.RegisterFighter(this);
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
        fighterRef = fighter.GetComponent<Fighter>();
        fighterRef.Initialize(this, fightScene);
        if (GameWizard.Instance.IsNetworkedGame())
        {
            fighterRef.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        }
        return fighterRef;
    }
}