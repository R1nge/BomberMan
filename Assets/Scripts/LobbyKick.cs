using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyKick : NetworkBehaviour
{
    [SerializeField] private Button kick;
    private LobbyManager _lobbyManager;

    private void Awake()
    {
        _lobbyManager = FindObjectOfType<LobbyManager>();
        if (!IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += LoadMainMenu;
            kick.onClick.AddListener(Kick);
        }
    }

    public override void OnNetworkSpawn() => kick.gameObject.SetActive(IsServer && !IsOwner);

    private void Kick()
    {
        if (IsServer)
        {
            _lobbyManager.OnClientDisconnect(GetComponent<NetworkObject>().OwnerClientId);
        }

        NetworkManager.Singleton.DisconnectClient(GetComponent<NetworkObject>().OwnerClientId);
    }

    private void LoadMainMenu(ulong obj)
    {
        if (IsServer || !IsOwner) return;
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        kick.onClick.RemoveAllListeners();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientDisconnectCallback -= LoadMainMenu;
    }
}