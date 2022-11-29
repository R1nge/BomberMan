using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : NetworkBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject tile, destructable, wall;
    [SerializeField] private int width, height;
    [SerializeField] private Vector3 tileOffset, destructableOffset;
    [SerializeField] private float tileSize, destructableSize;
    private bool _spawnObstacle;
    private SpawnPositions _spawnPositions;

    private void Awake() => _spawnPositions = FindObjectOfType<SpawnPositions>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _spawnPositions.SetSpawnPositions((width - 1) * tileSize, (height - 1) * tileSize);
        SpawnGrid();
        SpawnDestructables();
        SpawnWalls();
    }

    private void SpawnGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var inst = Instantiate(tile, (new Vector3(x, 0, z) + tileOffset) * tileSize, quaternion.identity);
                inst.GetComponent<NetworkObject>().Spawn(true);
                inst.transform.parent = parent;
            }
        }
    }

    private void SpawnDestructables()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                _spawnObstacle = Mathf.FloorToInt(Random.Range(0, 2)) == 0;
                if (!_spawnObstacle) continue;

                if (x == 0 && z == 0) continue;
                if (x == 1 && z == 0) continue;
                if (x == 0 && z == 1) continue;

                if (x == width - 1 && z == 0) continue;
                if (x == width - 2 && z == 0) continue;
                if (x == width - 1 && z == 1) continue;

                if (x == 0 && z == height - 1) continue;
                if (x == 1 && z == height - 1) continue;
                if (x == 0 && z == height - 2) continue;

                if (x == width - 1 && z == height - 1) continue;
                if (x == width - 2 && z == height - 1) continue;
                if (x == width - 1 && z == height - 2) continue;

                var inst = Instantiate(destructable, (new Vector3(x, 0, z) + destructableOffset) * destructableSize,
                    quaternion.identity);
                inst.GetComponent<NetworkObject>().Spawn(true);
                inst.transform.parent = parent;
            }
        }
    }

    private void SpawnWalls()
    {
        for (int x = -1; x < width + 1; x++)
        {
            for (int z = -1; z < height + 1; z++)
            {
                if (x < width + 1 && z == height)
                {
                    SpawnWall(x, z);
                }
                else if (x < width + 1 && z == -1)
                {
                    SpawnWall(x, z);
                }
                else if (x == -1 && z < height + 1)
                {
                    SpawnWall(x, z);
                }
                else if (x == width && z < height + 1)
                {
                    SpawnWall(x, z);
                }
            }
        }
    }

    private void SpawnWall(int x, int z)
    {
        var inst = Instantiate(wall, (new Vector3(x, 0, z) + destructableOffset) * tileSize,
            quaternion.identity);
        inst.GetComponent<NetworkObject>().Spawn(true);
        inst.transform.parent = parent;
    }
}