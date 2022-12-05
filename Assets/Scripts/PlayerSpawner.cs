using System;
using BayatGames.SaveGameFree;
using Character;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    private NetworkVariable<int> _playersAmount;
    private SpawnPositions _spawnPositions;
    private GameState _gameState;
    private PlayerSkins _skins;

    private void Awake()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        _playersAmount = new NetworkVariable<int>();
        _skins = FindObjectOfType<PlayerSkins>();
        _spawnPositions = FindObjectOfType<SpawnPositions>();
        _gameState = FindObjectOfType<GameState>();
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

    private void OnClientDisconnect(ulong obj)
    {
        if (IsServer)
        {
            _playersAmount.Value--;
        }
    }

    private void SpawnPlayer(int skinIndex, ulong ID)
    {
        if (IsServer)
        {
            var player = Instantiate(_skins.GetPlayerPrefab(skinIndex),
                _spawnPositions.GetPositions()[_playersAmount.Value],
                Quaternion.identity);
            player.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
            player.GetComponent<NetworkObject>().transform.position =
                _spawnPositions.GetPositions()[_playersAmount.Value];
            _playersAmount.Value++;
        }
        else
        {
            SpawnPlayerServerRpc(skinIndex);
        }
    }

    public void Despawn(ulong ID)
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[ID].Despawn();
            _playersAmount.Value--;

            if (_playersAmount.Value <= 1)
            {
                var controllers = FindObjectsOfType<MovementController>();
                for (int i = 0; i < controllers.Length; i++)
                {
                    if (controllers[i].GetComponent<NetworkObject>().IsSpawned)
                    {
                        var winPlayer =
                            NetworkManager.Singleton.SpawnManager.SpawnedObjects[controllers[i].NetworkObjectId];
                        var winName = winPlayer.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text;
                        if (winPlayer != null)
                        {
                            _gameState.WinServerRpc(winName);
                        }

                        break;
                    }

                    _gameState.GameoverServerRpc();
                }
            }
        }
        else
        {
            DespawnServerRpc(ID);
        }
    }

    [ServerRpc]
    private void DespawnServerRpc(ulong ID)
    {
        Despawn(ID);
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
    }
}