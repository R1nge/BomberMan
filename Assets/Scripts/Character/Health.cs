using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class Health : NetworkBehaviour, IDamageable
    {
        [SerializeField] private NetworkVariable<int> health;
        private PlayerSpawner _playerSpawner;

        private void Awake()
        {
            _playerSpawner = FindObjectOfType<PlayerSpawner>();
        }

        public void TakeDamage(int amount)
        {
            health.Value -= amount;
            if (health.Value <= 0)
            {
                _playerSpawner.Despawn(GetComponent<NetworkObject>().NetworkObjectId);
                Debug.LogWarning("Player died", this);
            }
        }
    }
}