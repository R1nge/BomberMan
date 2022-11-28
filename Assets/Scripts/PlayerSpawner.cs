using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private NetworkVariable<int> players;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SpawnPlayer(players.Value);
    }

    private void SpawnPlayer(int index)
    {
        if (!IsServer)
        {
            SpawnPlayerServerRpc(index);
        }
        else
        {
            var player = Instantiate(playerPrefab, spawnPoints[index].position, Quaternion.identity);
            //Quick and dirty hack, but i'll leave it for now
            player.GetComponent<NetworkObject>().SpawnWithOwnership((ulong) players.Value);
            players.Value++;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(int index)
    {
        SpawnPlayer(index);
        players.Value++;
    }
}