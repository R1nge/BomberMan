using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class Health : NetworkBehaviour, IDamageable
    {
        [SerializeField] private NetworkVariable<int> health;
        [SerializeField] private PlayerUI playerUI;
        private PlayerSpawner _playerSpawner;

        private void Awake()
        {
            _playerSpawner = FindObjectOfType<PlayerSpawner>();
            health.OnValueChanged += playerUI.UpdateHealth;
        }

        private void Start() => playerUI.UpdateHealth(health.Value, health.Value);

        public void TakeDamage(int amount)
        {
            health.Value -= amount;
            if (health.Value <= 0)
            {
                _playerSpawner.Despawn(GetComponent<NetworkObject>().NetworkObjectId);
                Debug.LogWarning("Player died", this);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseHealthServerRpc(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogError("Trying to add negative/zero value", this);
                return;
            }

            health.Value += amount;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            health.OnValueChanged += playerUI.UpdateHealth;
        }
    }
}