﻿using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : NetworkBehaviour
{
    [SerializeField] private int minWidth, minHeight;
    [SerializeField] private int maxWidth, maxHeight;
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject tile, destructable, borderWall, wall;
    [SerializeField] private Vector3 tileOffset, destructableOffset;
    [SerializeField] private float tileSize, destructableSize;
    private int _width, _height;
    private bool _spawnObstacle;
    private SpawnPositions _spawnPositions;

    private void Awake() => _spawnPositions = FindObjectOfType<SpawnPositions>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _width = GetRandomOddNumber(minWidth, maxWidth);
        _height = GetRandomOddNumber(minHeight, maxHeight);
        _spawnPositions.SetSpawnPositions((_width - 1) * tileSize, (_height - 1) * tileSize);
        SpawnGrid();
        SpawnWalls();
        SpawnDestructables();
        SpawnBorders();
    }

    private void SpawnGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                var inst = Instantiate(tile, (new Vector3(x, 0, z) + tileOffset) * tileSize, quaternion.identity);
                inst.GetComponent<NetworkObject>().Spawn(true);
                inst.transform.parent = parent;
            }
        }
    }

    private void SpawnDestructables()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                _spawnObstacle = Mathf.FloorToInt(Random.Range(0, 2)) == 0;
                if (!_spawnObstacle) continue;

                if (x % 2 == 1 && z % 2 == 1) continue;

                if (x == 0 && z == 0) continue;
                if (x == 1 && z == 0) continue;
                if (x == 0 && z == 1) continue;

                if (x == _width - 1 && z == 0) continue;
                if (x == _width - 2 && z == 0) continue;
                if (x == _width - 1 && z == 1) continue;

                if (x == 0 && z == _height - 1) continue;
                if (x == 1 && z == _height - 1) continue;
                if (x == 0 && z == _height - 2) continue;

                if (x == _width - 1 && z == _height - 1) continue;
                if (x == _width - 2 && z == _height - 1) continue;
                if (x == _width - 1 && z == _height - 2) continue;

                var inst = Instantiate(destructable, (new Vector3(x, 0, z) + destructableOffset) * destructableSize,
                    quaternion.identity);
                inst.GetComponent<NetworkObject>().Spawn(true);
                inst.transform.parent = parent;
            }
        }
    }

    private void SpawnBorders()
    {
        for (int x = -1; x < _width + 1; x++)
        {
            for (int z = -1; z < _height + 1; z++)
            {
                if (x < _width + 1 && z == _height)
                {
                    SpawnBorderWall(x, z);
                }
                else if (x < _width + 1 && z == -1)
                {
                    SpawnBorderWall(x, z);
                }
                else if (x == -1 && z < _height + 1)
                {
                    SpawnBorderWall(x, z);
                }
                else if (x == _width && z < _height + 1)
                {
                    SpawnBorderWall(x, z);
                }
            }
        }
    }

    private void SpawnWalls()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (x % 2 == 1 && y % 2 == 1)
                {
                    SpawnWall(x, y);
                }
            }
        }
    }

    private void SpawnBorderWall(int x, int z)
    {
        var inst = Instantiate(borderWall, (new Vector3(x, 0, z) + destructableOffset) * tileSize,
            quaternion.identity);
        inst.GetComponent<NetworkObject>().Spawn(true);
        inst.transform.parent = parent;
    }

    private void SpawnWall(int x, int z)
    {
        var inst = Instantiate(wall, (new Vector3(x, 0, z) + destructableOffset) * tileSize,
            quaternion.identity);
        inst.GetComponent<NetworkObject>().Spawn(true);
        inst.transform.parent = parent;
    }

    private int GetRandomOddNumber(int min, int max)
    {
        var num = Random.Range(min, max + 1);
        if (num % 2 == 0)
        {
            num++;
        }

        return num;
    }
}