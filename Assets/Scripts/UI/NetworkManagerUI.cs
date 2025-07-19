using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    public void Host()
    {
        GameWizard.Instance.SetGameAsNetworked();
        NetworkManager.Singleton.StartHost();
    }

    public void Join()
    {
        GameWizard.Instance.SetGameAsNetworked();
        NetworkManager.Singleton.StartClient();
    }
}