using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

namespace Character
{
    public class CharacterBlock : NetworkBehaviour
    {
        [SerializeField] private float distance;
        [SerializeField] private NetworkVariable<int> digAmount;
        [SerializeField] private NetworkVariable<int> blockAmount;
        private MapGenerator _mapGenerator;
        private BlocksUI _blocksUI;

        private void Awake()
        {
            _mapGenerator = FindObjectOfType<MapGenerator>();
            _blocksUI = GetComponent<BlocksUI>();
            digAmount.OnValueChanged += (_, newValue) => { _blocksUI.UpdateDig(newValue); };
            blockAmount.OnValueChanged += (_, newValue) => { _blocksUI.UpdateBlock(newValue); };
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseDigAmountServerRpc() => digAmount.Value++;

        public void OnPlaceBlock(InputValue value)
        {
            if (!IsOwner) return;
            if (blockAmount.Value <= 0) return;
            var distanceMultiplier = 1.55f;
            if (!Physics.Raycast(transform.position, transform.forward, distance * distanceMultiplier))
            {
                SpawnBlock(transform.position + transform.forward * distance * distanceMultiplier);
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
                var inst = Addressables.InstantiateAsync(_mapGenerator.GetCurrentMapConfig().playerWall,
                    pos, Quaternion.identity);
                inst.Completed += handle =>
                {
                    handle.Result.GetComponent<PlaceInGridClass>().PlaceInGrid();
                    handle.Result.GetComponent<NetworkObject>().Spawn(true);
                };

                blockAmount.Value--;
            }
            else
            {
                SpawnBlockServerRpc(pos);
            }
        }

        [ServerRpc]
        private void SpawnBlockServerRpc(Vector3 pos) => SpawnBlock(pos);

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