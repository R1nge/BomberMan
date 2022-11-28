using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private NetworkVariable<int> players;
    private NetworkList<Vector3> _positions;

    private void Awake()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            players = new NetworkVariable<int>();
            _positions = new NetworkList<Vector3>();
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                print("Position " + i);
                _positions.Add(spawnPositions[i].position);
            }
        }

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (IsServer)
        {
            //Quick and dirty hack, but i'll leave it for now
            var player = Instantiate(playerPrefab, spawnPositions[players.Value].position, Quaternion.identity);
            player.GetComponent<NetworkObject>().SpawnWithOwnership((ulong) players.Value);
            player.GetComponent<NetworkObject>().transform.position = spawnPositions[players.Value].position;

            players.Value++;
        }
        else
        {
            SpawnPlayerServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc() => SpawnPlayer();

    public override void OnDestroy()
    {
        base.OnDestroy();
        players.Dispose();
    }
}