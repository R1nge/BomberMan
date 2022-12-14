using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class DamageController : MonoBehaviour
    {
        [SerializeField] private GameObject hitSound;
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.OnTakenDamage += OnTakenDamage;
        }

        private void OnTakenDamage() => OnTakenDamageServerRpc();

        [ServerRpc(RequireOwnership = false)]
        private void OnTakenDamageServerRpc() => OnTakenDamageClientRpc();

        [ClientRpc]
        private void OnTakenDamageClientRpc() => Instantiate(hitSound);

        private void OnDestroy() => _health.OnTakenDamage -= OnTakenDamage;
    }
}