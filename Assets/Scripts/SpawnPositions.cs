using Unity.Netcode;
using UnityEngine;

public class SpawnPositions : NetworkBehaviour
{
    [SerializeField] private float y;
    private NetworkList<Vector3> _networkPositions;

    private void Awake() => _networkPositions = new NetworkList<Vector3>();

    public void SetSpawnPositions(int width, int height)
    {
        if (IsServer)
        {
            _networkPositions.Add(new Vector3(0, y, 0));
            _networkPositions.Add(new Vector3(0, y, height));
            _networkPositions.Add(new Vector3(width, y, 0));
            _networkPositions.Add(new Vector3(width, y, height));
        }
    }

    public NetworkList<Vector3> GetPositions() => _networkPositions;
}