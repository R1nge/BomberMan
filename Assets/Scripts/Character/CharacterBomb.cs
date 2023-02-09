using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class CharacterBomb : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<int> bombAmount, maxBombAmount;
        private GameState _gameState;
        private PlayerBombs _bombs;
        private int _currentBomb;

        public event Action<int, int> OnBombPlaced;

        private void Awake()
        {
            _gameState = FindObjectOfType<GameState>();
            _bombs = FindObjectOfType<PlayerBombs>();
            _currentBomb = PlayerPrefs.GetInt("Bomb", 0);
        }

        public override void OnNetworkSpawn()
        {
            bombAmount.OnValueChanged += (_, newValue) =>
            {
                OnBombPlaced?.Invoke(newValue, maxBombAmount.Value);
            };
        }

        private void Start()
        {
            OnBombPlaced?.Invoke(bombAmount.Value, maxBombAmount.Value);
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
                    Spawn(_currentBomb, NetworkObject.OwnerClientId);
                }
                else
                {
                    SpawnServerRpc(_currentBomb);
                }
                
                OnBombPlaced?.Invoke(bombAmount.Value, maxBombAmount.Value);
            }
        }

        private void Spawn(int bombIndex, ulong ID)
        {
            if (IsServer && CanSpawn())
            {
                bombAmount.Value--;
                var bomb = Instantiate(_bombs.GetBombPrefab(bombIndex), transform.position, Quaternion.identity);
                bomb.GetComponent<PlaceInGridClass>().PlaceInGrid();
                var net = bomb.GetComponent<NetworkObject>();
                net.SpawnWithOwnership(ID, true);
                net.DontDestroyWithOwner = true;
                net.GetComponent<Bomb>().OnBombExploded += ResetSpawn;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(int bombIndex, ServerRpcParams rpcParams = default)
        {
            Spawn(bombIndex, rpcParams.Receive.SenderClientId);
        }

        private bool CanSpawn()
        {
            var coll = new Collider[4];

            var size = Physics.OverlapBoxNonAlloc(transform.position, transform.localScale / 4, coll,
                Quaternion.identity);

            for (int i = 0; i < size; i++)
            {
                if (coll[i].TryGetComponent(out Bomb _))
                {
                    return false;
                }
            }

            return true;
        }

        private void ResetSpawn(Bomb bomb)
        {
            if (!IsSpawned) return;
            bomb.OnBombExploded -= ResetSpawn;
            ResetSpawnServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void ResetSpawnServerRpc()
        {
            if (bombAmount.Value >= maxBombAmount.Value) return;
            bombAmount.Value++;
        }
    }
}