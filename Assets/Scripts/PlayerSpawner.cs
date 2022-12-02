using System.Collections;
using BayatGames.SaveGameFree;
using Character;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> players;
    private SpawnPositions _spawnPositions;
    private GameState _gameState;
    private PlayerSkins _skins;

    private void Awake()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            players = new NetworkVariable<int>();
        }

        _skins = FindObjectOfType<PlayerSkins>();
        _spawnPositions = FindObjectOfType<SpawnPositions>();
        _gameState = FindObjectOfType<GameState>();
    }

    public override void OnNetworkSpawn()
    {
        StartCoroutine(Wait_C());
    }

    private IEnumerator Wait_C()
    {
        yield return new WaitForSeconds(1f);
        SpawnPlayerServerRpc(SaveGame.Load("Skin", 0));
    }

    private void SpawnPlayer(int skinIndex, ulong ID)
    {
        if (IsServer)
        {
            //TODO: load skin that player has chosen
            //Quick and dirty hack, but i'll leave it for now
            var player = Instantiate(_skins.GetSkin(skinIndex), _spawnPositions.GetPositions()[players.Value],
                Quaternion.identity);
            player.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
            player.GetComponent<NetworkObject>().transform.position = _spawnPositions.GetPositions()[players.Value];

            players.Value++;
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
            players.Value--;

            if (players.Value <= 1)
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
        players.Dispose();
    }
}