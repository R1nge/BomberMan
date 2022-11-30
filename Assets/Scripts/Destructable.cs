using Powerups;
using Unity.Netcode;
using UnityEngine;

public class Destructable : NetworkBehaviour, IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private NetworkVariable<float> dropChance;
    [SerializeField] private Powerup[] drops;
    private NetworkVariable<int> _index = new NetworkVariable<int>();

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            if (!IsServer)
            {
                SpawnDropServerRpc();
                DestroyServerRpc();
            }
            else
            {
                SpawnDropServerRpc();
                DestroyServerRpc();
            }
        }
    }

    [ServerRpc]
    private void DestroyServerRpc() => GetComponent<NetworkObject>().Despawn();

    [ServerRpc(RequireOwnership = false)]
    private void SpawnDropServerRpc()
    {
        //TODO: fix drop chance
        if (Random.value < 1 - dropChance.Value) return;
        _index.Value = Random.Range(0, drops.Length);
        var drop = Instantiate(drops[_index.Value], transform.position, Quaternion.identity);
        drop.GetComponent<NetworkObject>().Spawn(true);
    }
}