using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button host, client;

    private void Awake()
    {
        host.onClick.AddListener(() => { NetworkManager.Singleton.StartHost(); });

        client.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); });
    }
}