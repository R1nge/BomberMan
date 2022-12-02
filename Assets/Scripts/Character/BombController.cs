using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class BombController : NetworkBehaviour
    {
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private NetworkVariable<int> bombAmount, maxBombAmount;
        [SerializeField] private PlayerUI playerUI;

        private void Awake() => bombAmount.OnValueChanged += playerUI.UpdateBombs;

        private void Start() => playerUI.UpdateBombs(bombAmount.Value, bombAmount.Value);

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseBombAmountServerRpc(int value)
        {
            if (bombAmount.Value >= maxBombAmount.Value) return;
            bombAmount.Value += value;
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
            if (bombAmount.Value >= maxBombAmount.Value) return;
            bombAmount.Value++;
        }
    }
}