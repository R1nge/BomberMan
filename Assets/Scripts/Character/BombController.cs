using BayatGames.SaveGameFree;
using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class BombController : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<int> bombAmount, maxBombAmount;
        [SerializeField] private PlayerUI playerUI;
        private Bombs _bombs;

        private void Awake() => _bombs = FindObjectOfType<Bombs>();

        public override void OnNetworkSpawn()
        {
            bombAmount.OnValueChanged += (value, newValue) =>
            {
                if (playerUI == null) return;
                playerUI.UpdateBombs(bombAmount.Value, maxBombAmount.Value);
            };
        }

        private void Start() => playerUI.UpdateBombs(bombAmount.Value, maxBombAmount.Value);

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
                if (IsServer)
                {
                    Spawn(transform.position, SaveGame.Load("Bomb", 0));
                }
                else
                {
                    SpawnServerRpc(transform.position, SaveGame.Load("Bomb", 0));
                }
            }
        }

        private void Spawn(Vector3 position, int index)
        {
            if (IsServer)
            {
                bombAmount.Value--;
                var bomb = Instantiate(_bombs.GetBomb(index), position, Quaternion.identity);
                var net = bomb.GetComponent<NetworkObject>();
                net.Spawn(true);
                net.GetComponent<PlaceInGrid>().PlaceInGridServerRpc();
                var countdown = bomb.GetComponent<Bomb>().ExplodeDelay;
                Invoke(nameof(ResetSpawnServerRpc), countdown);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(Vector3 position, int index) => Spawn(position, index);

        [ServerRpc(RequireOwnership = false)]
        private void ResetSpawnServerRpc()
        {
            if (bombAmount.Value >= maxBombAmount.Value) return;
            bombAmount.Value++;
        }
    }
}