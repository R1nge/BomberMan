using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class Health : NetworkBehaviour, IDamageable
    {
        [SerializeField] private NetworkVariable<int> health;
        private PlayerSpawner _playerSpawner;
        private PlayerUI _playerUI;

        private void Awake()
        {
            _playerSpawner = FindObjectOfType<PlayerSpawner>();
            _playerUI = GetComponent<PlayerUI>();
        }

        public override void OnNetworkSpawn() => _playerUI.UpdateHealth(health.Value);

        public void TakeDamage(int amount)
        {
            health.Value -= amount;
            if (health.Value <= 0)
            {
                _playerSpawner.Despawn(GetComponent<NetworkObject>().NetworkObjectId);
                Debug.LogWarning("Player died", this);
            }
        }

        [ServerRpc]
        public void IncreaseHealthServerRpc(int amount)
        {
            health.Value += amount;
            _playerUI.UpdateHealth(amount);
        }
    }
}