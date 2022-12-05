using System;
using Powerups;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : NetworkBehaviour, IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private NetworkVariable<float> dropChance;
    [SerializeField] private Powerup[] drops;
    [SerializeField] private Vector3 dropOffset;
    private NetworkVariable<int> _dropIndex;

    private void Awake() => _dropIndex = new NetworkVariable<int>();

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
        _dropIndex.Value = Random.Range(0, drops.Length);
        var drop = Instantiate(drops[_dropIndex.Value], transform.position + dropOffset, Quaternion.identity);
        drop.GetComponent<NetworkObject>().Spawn(true);
    }
}