using BayatGames.SaveGameFree;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class BombController : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<int> bombAmount, maxBombAmount;
        private PlayerUI _playerUI;
        private GameState _gameState;
        private Bombs _bombs;
        private int _currentBomb;

        private void Awake()
        {
            _playerUI = GetComponent<PlayerUI>();
            _gameState = FindObjectOfType<GameState>();
            _bombs = FindObjectOfType<Bombs>();
            _currentBomb = SaveGame.Load("Bomb", 0);
        }

        public override void OnNetworkSpawn()
        {
            bombAmount.OnValueChanged += (value, newValue) =>
            {
                if (_playerUI == null) return;
                _playerUI.UpdateBombs(bombAmount.Value, maxBombAmount.Value);
            };
        }

        private void Start()
        {
            if (_playerUI == null) return;
            _playerUI.UpdateBombs(bombAmount.Value, maxBombAmount.Value);
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseBombAmountServerRpc(int value)
        {
            if (bombAmount.Value >= maxBombAmount.Value) return;
            bombAmount.Value += value;
        }

        public void OnPlaceBomb(InputValue value)
        {
            if (!IsOwner) return;
            if (_gameState.GameEnded.Value) return;
            if (!_gameState.GameStarted.Value) return;
            if (bombAmount.Value > 0)
            {
                if (IsServer)
                {
                    Spawn(NetworkObject.OwnerClientId);
                }
                else
                {
                    SpawnServerRpc();
                }
            }
        }

        private void Spawn(ulong ID)
        {
            if (IsServer && CanSpawn())
            {
                bombAmount.Value--;
                var bomb = Instantiate(_bombs.GetBomb(_currentBomb), transform.position, Quaternion.identity);
                bomb.GetComponent<PlaceInGridClass>().PlaceInGrid();
                var net = bomb.GetComponent<NetworkObject>();
                net.SpawnWithOwnership(ID, true);
                net.GetComponent<Bomb>().OnBombExploded += ResetSpawnServerRpc;
            }
        }

        private bool CanSpawn()
        {
            var coll = new Collider[4];

            var size = Physics.OverlapBoxNonAlloc(transform.position, transform.localScale / 4, coll,
                Quaternion.identity);
            for (int i = 0;
                 i < size;
                 i++)
            {
                if (coll[i].TryGetComponent(out Bomb _))
                {
                    return false;
                }
            }

            return true;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(ServerRpcParams rpcParams = default)
        {
            Spawn(rpcParams.Receive.SenderClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ResetSpawnServerRpc()
        {
            if (bombAmount.Value >= maxBombAmount.Value) return;
            bombAmount.Value++;
        }
    }
}