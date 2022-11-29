using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : NetworkBehaviour
{
    [SerializeField] private GameObject tile, destructable;
    [SerializeField] private int width, height;
    [SerializeField] private Vector3 tileOffset, destructableOffset;
    [SerializeField] private float tileSize, destructableSize;
    private bool _spawnObstacle;
    private SpawnPositions _spawnPositions;

    private void Awake() => _spawnPositions = FindObjectOfType<SpawnPositions>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _spawnPositions.SetSpawnPositions(width,height);
        SpawnGrid();
        SpawnDestructables();
    }

    private void SpawnGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var inst = Instantiate(tile, (new Vector3(x, 0, z) + tileOffset) * tileSize, quaternion.identity);
                inst.GetComponent<NetworkObject>().Spawn();
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
                inst.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}