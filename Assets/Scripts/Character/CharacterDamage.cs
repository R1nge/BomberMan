using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class CharacterDamage : MonoBehaviour
    {
        [SerializeField] private GameObject hitSound;
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.OnTakenDamage += OnTakenDamage;
        }

        private void OnTakenDamage(int _) => OnTakenDamageServerRpc();

        [ServerRpc(RequireOwnership = false)]
        private void OnTakenDamageServerRpc()
        {
            var sound = Instantiate(hitSound, transform.position, quaternion.identity);
            sound.GetComponent<NetworkObject>().Spawn();
        }

        private void OnDestroy() => _health.OnTakenDamage -= OnTakenDamage;
    }
}