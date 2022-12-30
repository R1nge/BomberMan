using BayatGames.SaveGameFree;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private GameObject start;
    [SerializeField] private Transform[] positions;
    private NetworkVariable<int> _playersAmount;
    private PlayerSkins _skins;

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        _playersAmount = new NetworkVariable<int>(1);
        _skins = FindObjectOfType<PlayerSkins>();
    }

    private void OnClientConnected(ulong obj)
    {
        if (!IsServer)
        {
            SpawnPreviewServerRpc(SaveGame.Load("Skin", 0));
            return;
        }

        _playersAmount.Value++;
        CheckIsFull(obj);
        start.SetActive(_playersAmount.Value != 1);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            start.gameObject.SetActive(false);
            return;
        }

        SpawnPreviewServerRpc(SaveGame.Load("Skin", 0));
        start.SetActive(_playersAmount.Value != 1);
    }

    public void OnClientDisconnect(ulong obj)
    {
        if (!IsServer) return;
        _playersAmount.Value--;
        start.SetActive(_playersAmount.Value != 1);
    }

    private void CheckIsFull(ulong ID)
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
        var player = Instantiate(_skins.GetPreviewPrefab(skinIndex), positions[_playersAmount.Value - 1].position, rot);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
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
        NetworkManager.Singleton.OnClientConnectedCallback -= CheckIsFull;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
    }
}