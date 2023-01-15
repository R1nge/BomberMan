using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyKick : NetworkBehaviour
{
    [SerializeField] private Button kick;

    private void Awake()
    {
        if (!IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += LoadMainMenu;
            kick.onClick.AddListener(Kick);
        }
    }

    public override void OnNetworkSpawn() => kick.gameObject.SetActive(IsServer && !IsOwner);

    private void Kick()
    {
        NetworkManager.Singleton.DisconnectClient(GetComponent<NetworkObject>().OwnerClientId);
    }

    private void LoadMainMenu(ulong obj)
    {
        if (IsServer || !IsOwner) return;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        kick.onClick.RemoveAllListeners();
    }
}