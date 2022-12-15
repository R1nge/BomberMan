using Unity.Netcode;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MapGenerator : NetworkBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private MapConfig[] maps;
    [SerializeField] private GameObject spawnInCenter;
    private int _width, _height;
    private bool _spawnObstacle;
    private int _mapIndex;
    private SpawnPositions _spawnPositions;
    private AsyncOperationHandle<GameObject> _tile, _destructable, _wall, _border;

    public MapConfig GetCurrentMapConfig() => maps[_mapIndex];

    private void Awake()
    {
        _spawnPositions = FindObjectOfType<SpawnPositions>();
        _mapIndex = Random.Range(0, maps.Length);
        _width = Mathf.FloorToInt(maps[_mapIndex].GetSize().x);
        _height = Mathf.FloorToInt(maps[_mapIndex].GetSize().y);
    }

    private void Start()
    {
        if (!IsServer) return;
        _spawnPositions.SetSpawnPositions((_width - 1) * maps[_mapIndex].tileSize,
            (_height - 1) * maps[_mapIndex].tileSize);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        parent = Instantiate(parent);
        parent.GetComponent<NetworkObject>().Spawn(true);
        SpawnTiles();
        SpawnWalls();
        SpawnDestructables();
        SpawnBorders();
        Spawn(spawnInCenter, _width / 2, _height / 2, new Vector3(0, maps[_mapIndex].tileOffset.y + 2, 0),
            maps[_mapIndex].tileSize, Vector3.zero);
    }

    private void SpawnTiles()
    {
        var map = maps[_mapIndex];
        _tile = map.tile.LoadAssetAsync<GameObject>();
        _tile.Completed += handle =>
        {
            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Spawn(handle.Result, x, z, map.tileOffset, map.tileSize, map.tileRotation);
                    }
                }
            }
        };
    }

    private void SpawnDestructables()
    {
        var map = maps[_mapIndex];
        _destructable = map.destructable.LoadAssetAsync<GameObject>();
        _destructable.Completed += handle =>
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

                    if (x == _width / 2 && z == _height / 2) continue;


                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Spawn(handle.Result, x, z, map.obstacleOffset,
                            map.obstacleSize, map.obstacleRotation);
                    }
                }
            }
        };
    }

    private void SpawnBorders()
    {
        var map = maps[_mapIndex];
        _border = map.border.LoadAssetAsync<GameObject>();
        _border.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                for (int x = -1; x < _width + 1; x++)
                {
                    for (int z = -1; z < _height + 1; z++)
                    {
                        if (x < _width + 1 && z == _height)
                        {
                            Spawn(handle.Result, x, z, maps[_mapIndex].borderOffset,
                                map.borderSize, map.topRotation);
                        }
                        else if (x < _width + 1 && z == -1)
                        {
                            Spawn(handle.Result, x, z, maps[_mapIndex].borderOffset,
                                map.borderSize, -map.topRotation);
                        }
                        else if (x == -1 && z < _height + 1)
                        {
                            Spawn(handle.Result, x, z, maps[_mapIndex].borderOffset,
                                map.borderSize, map.leftRotation);
                        }
                        else if (x == _width && z < _height + 1)
                        {
                            Spawn(handle.Result, x, z, maps[_mapIndex].borderOffset,
                                map.borderSize, -map.leftRotation);
                        }
                    }
                }
            }
        };
    }

    private void SpawnWalls()
    {
        var map = maps[_mapIndex];
        _wall = map.wall.LoadAssetAsync<GameObject>();
        _wall.Completed += handle =>
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (x % 2 == 1 && y % 2 == 1)
                    {
                        if (x == _width / 2 && y == _height / 2) continue;

                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            Spawn(handle.Result, x, y, map.wallOffset, map.wallSize, map.wallRotation);
                        }
                    }
                }
            }
        };
    }

    private void Spawn(GameObject go, int x, int z, Vector3 offset, float size, Vector3 rot)
    {
        var inst = Instantiate(go, new Vector3(x, 0, z) * size + offset,
            Quaternion.Euler(rot));
        inst.GetComponent<NetworkObject>().Spawn(true);
        inst.transform.parent = parent.transform;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        var map = maps[_mapIndex];
        map.tile.ReleaseAsset();
        map.wall.ReleaseAsset();
        map.playerWall.ReleaseAsset();
        map.border.ReleaseAsset();
        map.destructable.ReleaseAsset();
    }
}