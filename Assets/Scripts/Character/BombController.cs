using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class BombController : NetworkBehaviour
    {
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private float countdown;
        private bool _canSpawn = true;

        private void Update()
        {
            if (!IsOwner) return;
            if (Input.GetKeyDown(KeyCode.Space) && _canSpawn)
            {
                Spawn(transform.position);
            }
        }

        private void Spawn(Vector3 position)
        {
            if (IsServer)
            {
                _canSpawn = false;
                var bomb = Instantiate(bombPrefab, position, Quaternion.identity);
                bomb.GetComponent<NetworkObject>().Spawn();
                Invoke(nameof(ResetSpawn), countdown);
            }
            else
            {
                SpawnServerRpc(position);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(Vector3 position) => Spawn(position);

        private void ResetSpawn() => _canSpawn = true;
    }
}