using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : NetworkBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private MapConfig[] maps;
    [SerializeField] private GameObject spawnInCenter;
    private int _width, _height;
    private bool _spawnObstacle;
    private int _mapIndex;
    private SpawnPositions _spawnPositions;

    private void Awake() => _spawnPositions = FindObjectOfType<SpawnPositions>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _mapIndex = Random.Range(0, maps.Length);
        _width = Mathf.FloorToInt(maps[_mapIndex].GetSize().x);
        _height = Mathf.FloorToInt(maps[_mapIndex].GetSize().y);
        _spawnPositions.SetSpawnPositions((_width - 1) * maps[_mapIndex].tileSize,
            (_height - 1) * maps[_mapIndex].tileSize);
        parent = Instantiate(parent);
        parent.GetComponent<NetworkObject>().Spawn(true);
        SpawnGrid();
        SpawnWalls();
        SpawnDestructables();
        SpawnBorders();
        Spawn(spawnInCenter, _width / 2, _height  / 2, new Vector3(0, maps[_mapIndex].tileOffset.y + 2, 0),
            maps[_mapIndex].tileSize);
    }

    private void SpawnGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                Spawn(maps[_mapIndex].tile, x, z, maps[_mapIndex].tileOffset, maps[_mapIndex].tileSize);
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

                if (x == _width / 2 && z == _height/ 2) continue;

                Spawn(maps[_mapIndex].destructable, x, z, maps[_mapIndex].destructableOffset,
                    maps[_mapIndex].destructableSize);
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
                    Spawn(maps[_mapIndex].borderWall, x, z, maps[_mapIndex].borderWallOffset,
                        maps[_mapIndex].borderWallSize);
                }
                else if (x < _width + 1 && z == -1)
                {
                    Spawn(maps[_mapIndex].borderWall, x, z, maps[_mapIndex].borderWallOffset,
                        maps[_mapIndex].borderWallSize);
                }
                else if (x == -1 && z < _height + 1)
                {
                    Spawn(maps[_mapIndex].borderWall, x, z, maps[_mapIndex].borderWallOffset,
                        maps[_mapIndex].borderWallSize);
                }
                else if (x == _width && z < _height + 1)
                {
                    Spawn(maps[_mapIndex].borderWall, x, z, maps[_mapIndex].borderWallOffset,
                        maps[_mapIndex].borderWallSize);
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
                    if (x == _width / 2 && y == _height/ 2) continue;
                    Spawn(maps[_mapIndex].wall, x, y, maps[_mapIndex].wallOffset, maps[_mapIndex].wallSize);
                }
            }
        }
    }

    private void Spawn(GameObject go, int x, int z, Vector3 offset, float size)
    {
        var inst = Instantiate(go, new Vector3(x, 0, z) * size + offset,
            quaternion.identity);
        inst.GetComponent<NetworkObject>().Spawn(true);
        inst.transform.parent = parent.transform;
    }
}