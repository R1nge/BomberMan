using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class BombController : NetworkBehaviour
    {
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private NetworkVariable<int> bombAmount, maxBombAmount;
        private NetworkVariable<bool> _canSpawn = new NetworkVariable<bool>(true);

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseBombAmountServerRpc(int value)
        {
            maxBombAmount.Value += value;
            bombAmount.Value = maxBombAmount.Value;
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (Input.GetKeyDown(KeyCode.Space) && bombAmount.Value > 0)
            {
                Spawn(transform.position);
            }
        }

        private void Spawn(Vector3 position)
        {
            if (IsServer)
            {
                _canSpawn.Value = false;
                bombAmount.Value--;
                var bomb = Instantiate(bombPrefab, position, Quaternion.identity);
                bomb.GetComponent<NetworkObject>().Spawn(true);
                var countdown = bomb.GetComponent<Bomb>().ExplodeDelay;
                Invoke(nameof(ResetSpawnServerRpc), countdown);
            }
            else
            {
                SpawnServerRpc(position);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(Vector3 position) => Spawn(position);

        [ServerRpc(RequireOwnership = false)]
        private void ResetSpawnServerRpc()
        {
            _canSpawn.Value = true;
            bombAmount.Value++;
        }
    }
}