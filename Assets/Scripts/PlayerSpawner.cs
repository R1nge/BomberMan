using System.Collections;
using Character;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject player1, player2, player3;
    [SerializeField] private NetworkVariable<int> players;
    private SpawnPositions _spawnPositions;
    private GameState _gameState;

    private void Awake()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            players = new NetworkVariable<int>();
        }

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
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (IsServer)
        {
            //Quick and dirty hack, but i'll leave it for now
            if (players.Value == 0)
            {
                var player = Instantiate(player1, _spawnPositions.GetPositions()[players.Value], Quaternion.identity);
                player.GetComponent<NetworkObject>().SpawnWithOwnership((ulong) players.Value, true);
                player.GetComponent<NetworkObject>().transform.position = _spawnPositions.GetPositions()[players.Value];
            }
            else if (players.Value == 1)
            {
                var player = Instantiate(player2, _spawnPositions.GetPositions()[players.Value], Quaternion.identity);
                player.GetComponent<NetworkObject>().SpawnWithOwnership((ulong) players.Value, true);
                player.GetComponent<NetworkObject>().transform.position = _spawnPositions.GetPositions()[players.Value];
            }
            else if (players.Value == 2)
            {
                var player = Instantiate(player3, _spawnPositions.GetPositions()[players.Value], Quaternion.identity);
                player.GetComponent<NetworkObject>().SpawnWithOwnership((ulong) players.Value, true);
                player.GetComponent<NetworkObject>().transform.position = _spawnPositions.GetPositions()[players.Value];
            }

            players.Value++;
        }
        else
        {
            SpawnPlayerServerRpc();
        }
    }

    //TODO: FIX!!!!!!!!!!!!!!
    public void Despawn(ulong ID)
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[ID].Despawn();
            players.Value--;
        }
        else
        {
            DespawnServerRpc(ID);
        }

        if (players.Value <= 1)
        {
            //TODO: fix nicknames
            var name = FindObjectOfType<MovementController>().transform.GetChild(1).GetChild(0)
                .GetComponent<TextMeshProUGUI>().text;
            _gameState.GameoverServerRpc(name);
        }
    }

    [ServerRpc]
    private void DespawnServerRpc(ulong ID)
    {
        Despawn(ID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc() => SpawnPlayer();

    public override void OnDestroy()
    {
        base.OnDestroy();
        players.Dispose();
    }
}