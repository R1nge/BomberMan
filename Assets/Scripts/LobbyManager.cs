using BayatGames.SaveGameFree;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private GameObject start;
    [SerializeField] private Transform[] positions;
    private NetworkVariable<int> _playersAmount = new NetworkVariable<int>();
    private PlayerSkins _skins;

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Check;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        _skins = FindObjectOfType<PlayerSkins>();
    }

    private void OnClientDisconnect(ulong obj)
    {
        if (IsServer)
        {
            _playersAmount.Value--;
        }
    }

    public override void OnNetworkSpawn()
    {
        SpawnPreviewServerRpc(SaveGame.Load("Skin", 0));
        if (!IsServer)
        {
            start.gameObject.SetActive(false);
        }
    }

    private void Check(ulong ID)
    {
        if (IsServer)
        {
            if (_playersAmount.Value >= 4)
            {
                NetworkManager.Singleton.DisconnectClient(ID);
                _playersAmount.Value = 4;
            }
        }
    }

    private void SpawnPreview(int skinIndex, ulong ID)
    {
        var rot = Quaternion.Euler(new Vector3(0, 180, 0));
        var player = Instantiate(_skins.GetPreviewPrefab(skinIndex), positions[_playersAmount.Value].position, rot);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
        _playersAmount.Value++;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPreviewServerRpc(int skinIndex, ServerRpcParams rpcParams = default)
    {
        SpawnPreview(skinIndex, rpcParams.Receive.SenderClientId);
    }

    public void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= Check;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
    }
}