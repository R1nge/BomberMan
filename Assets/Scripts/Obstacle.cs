using Unity.Netcode;
using UnityEngine;

public class Obstacle : NetworkBehaviour, IDamageable
{
    [SerializeField] private int health;
    //TODO: use float instead of net var 
    [SerializeField] private NetworkVariable<float> dropChance;
    [SerializeField] private Vector3 dropOffset;
    private NetworkVariable<int> _dropIndex;
    private MapGenerator _mapGenerator;

    private void Awake()
    {
        _dropIndex = new NetworkVariable<int>();
        _mapGenerator = FindObjectOfType<MapGenerator>();
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            SpawnDropServerRpc();
            DestroyServerRpc();
        }
    }

    [ServerRpc]
    private void DestroyServerRpc() => GetComponent<NetworkObject>().Despawn();

    [ServerRpc(RequireOwnership = false)]
    private void SpawnDropServerRpc()
    {
        if (Random.value < 1 - dropChance.Value) return;
        _dropIndex.Value = Random.Range(0, _mapGenerator.GetCurrentMapConfig().drops.Length);
        var drop = Instantiate(_mapGenerator.GetCurrentMapConfig().drops[_dropIndex.Value],
            transform.position + dropOffset, Quaternion.identity);
        drop.GetComponent<NetworkObject>().Spawn(true);
    }
}