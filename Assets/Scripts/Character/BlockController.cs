using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class BlockController : NetworkBehaviour
    {
        [SerializeField] private float distance;
        [SerializeField] private NetworkVariable<int> digAmount;
        [SerializeField] private NetworkVariable<int> blockAmount;
        private MapGenerator _mapGenerator;
        private GameObject _block;
        private BlocksUI _blocksUI;

        private void Awake()
        {
            _mapGenerator = FindObjectOfType<MapGenerator>();
            _blocksUI = GetComponent<BlocksUI>();
            digAmount.OnValueChanged += (value, newValue) => { _blocksUI.UpdateDig(newValue); };
            blockAmount.OnValueChanged += (value, newValue) => { _blocksUI.UpdateBlock(newValue); };
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            _block = _mapGenerator.GetCurrentMapConfig().playerWall;
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseDigAmountServerRpc() => digAmount.Value++;

        public void OnPlaceBlock(InputValue value)
        {
            if (!IsOwner) return;
            if (blockAmount.Value <= 0) return;
            if (!Physics.Raycast(transform.position, transform.forward, distance * 1.25f))
            {
                SpawnBlock(transform.position + transform.forward * distance);
            }
        }

        public void OnBreakBlock(InputValue value)
        {
            if (!IsOwner) return;
            if (digAmount.Value <= 0) return;
            if (Physics.Raycast(transform.position, transform.forward, out var hit, distance))
            {
                if (hit.transform.TryGetComponent(out NetworkObject networkObject))
                {
                    DestroyBlockServerRpc(networkObject);
                }
            }
        }

        private void SpawnBlock(Vector3 pos)
        {
            if (IsServer)
            {
                var inst = Instantiate(_block, pos, Quaternion.identity).GetComponent<NetworkObject>();
                inst.Spawn(true);
                inst.GetComponent<PlaceInGrid>().PlaceInGridServerRpc();
                blockAmount.Value--;
            }
            else
            {
                SpawnBlockServerRpc(pos);
            }
        }

        [ServerRpc]
        private void SpawnBlockServerRpc(Vector3 pos)
        {
            SpawnBlock(pos);
        }

        [ServerRpc]
        private void DestroyBlockServerRpc(NetworkObjectReference obj)
        {
            if (obj.TryGet(out NetworkObject networkObject))
            {
                if (networkObject.TryGetComponent(out Obstacle obstacle))
                {
                    obstacle.TakeDamage(1000);
                    blockAmount.Value++;
                    digAmount.Value--;
                }
            }
        }
    }
}