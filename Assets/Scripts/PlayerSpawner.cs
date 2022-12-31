using BayatGames.SaveGameFree;
using Character;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    private NetworkVariable<int> _playersAmount;
    private SpawnPositions _spawnPositions;
    private GameState _gameState;
    private PlayerSkins _skins;
    private NetworkList<int> _lockedPositions;
    private KillFeed _killFeed;

    public NetworkVariable<int> PlayersAmount => _playersAmount;

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        _playersAmount = new NetworkVariable<int>();
        _skins = FindObjectOfType<PlayerSkins>();
        _spawnPositions = FindObjectOfType<SpawnPositions>();
        _gameState = FindObjectOfType<GameState>();
        _lockedPositions = new NetworkList<int>();
        _killFeed = FindObjectOfType<KillFeed>();
    }

    private void OnClientConnected(ulong ID)
    {
        if (_playersAmount.Value >= 4)
        {
            if (IsServer)
            {
                NetworkManager.Singleton.DisconnectClient(ID);
                _playersAmount.Value = 4;
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    private void OnClientDisconnect(ulong ID)
    {
        if (!IsServer) return;
        if (GetNetworkObject(ID) == null) return;
        _playersAmount.Value--;
        if (_playersAmount.Value <= 1)
        {
            _gameState.GameOverServerRpc();
        }
    }

    private void Start() => _spawnPositions.GetPositions().OnListChanged += OnOnListChanged;

    private void OnOnListChanged(NetworkListEvent<Vector3> changeevent)
    {
        if (changeevent.Index == 3)
        {
            if (IsServer)
            {
                SpawnPlayer(SaveGame.Load("Skin", 0), NetworkManager.Singleton.LocalClientId);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) return;
        SpawnPlayerServerRpc(SaveGame.Load("Skin", 0));
    }

    private void SpawnPlayer(int skinIndex, ulong ID)
    {
        if (_gameState.GameStarted.Value) return;
        if (IsServer)
        {
            var pos = GetRandomCorner();
            var player = Instantiate(_skins.GetPlayerPrefab(skinIndex),
                _spawnPositions.GetPositions()[pos],
                Quaternion.identity);
            var net = player.GetComponent<NetworkObject>();
            net.SpawnAsPlayerObject(ID, true);
            net.transform.position =
                _spawnPositions.GetPositions()[pos];
            _playersAmount.Value++;
        }
        else
        {
            SpawnPlayerServerRpc(skinIndex);
        }
    }

    private int GetRandomCorner()
    {
        var pos = Random.Range(0, 4);
        if (_lockedPositions.Contains(pos))
        {
            return GetRandomCorner();
        }

        _lockedPositions.Add(pos);
        return pos;
    }

    public void Despawn(ulong who, ulong whom)
    {
        var player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(who);
        var d = player.transform.Find("Canvas/Nick").GetComponent<TextMeshProUGUI>().text;
        var player2 = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(whom);
        var c = player2.transform.Find("Canvas/Nick").GetComponent<TextMeshProUGUI>().text;
        _killFeed.DisplayKillServerRpc(d, c);

        if (IsServer)
        {
            NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(whom).Despawn();
            _playersAmount.Value--;

            if (_playersAmount.Value is <= 1 and > 0)
            {
                var controllers = FindObjectsOfType<CharacterMovement>();
                for (int i = 0; i < controllers.Length; i++)
                {
                    if (controllers[i].GetComponent<NetworkObject>().IsSpawned)
                    {
                        var winPlayer =
                            NetworkManager.Singleton.SpawnManager.SpawnedObjects[controllers[i].NetworkObjectId];
                        var winName = winPlayer.transform.Find("Canvas/Nick").GetComponent<TextMeshProUGUI>().text;
                        _gameState.WinServerRpc(winName);

                        break;
                    }
                }

                _gameState.GameOverServerRpc();
            }
            else if (_playersAmount.Value <= 0)
            {
                _gameState.TieServerRpc();
            }
        }
        else
        {
            DespawnServerRpc(who);
        }
    }

    [ServerRpc]
    private void DespawnServerRpc(ulong ID)
    {
        Despawn(ID, ID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(int skinIndex, ServerRpcParams rpcParams = default)
    {
        SpawnPlayer(skinIndex, rpcParams.Receive.SenderClientId);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _playersAmount?.Dispose();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
    }
}