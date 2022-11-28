using Unity.Netcode;
using UnityEngine;

public class Destructable : NetworkBehaviour, IDamageable
{
    [SerializeField] private int health;

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            if (!IsServer)
            {
                DestroyServerRpc();
            }
            else
            {
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }

    [ServerRpc]
    private void DestroyServerRpc() => GetComponent<NetworkObject>().Despawn();
}