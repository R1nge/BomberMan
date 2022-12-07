using System;
using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class BlockController : NetworkBehaviour
    {
        [SerializeField] private float distance;
        [SerializeField] private NetworkVariable<int> digAmount;
        [SerializeField] private NetworkVariable<int> blockCount;
        private MapGenerator _mapGenerator;
        private GameObject _block;

        private void Awake() => _mapGenerator = FindObjectOfType<MapGenerator>();

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            _block = _mapGenerator.GetCurrentMapConfig().playerWall;
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseDigAmountServerRpc() => digAmount.Value++;

        private void Update()
        {
            if (!IsOwner) return;
            if (Input.GetMouseButtonDown(0))
            {
                if (blockCount.Value <= 0) return;
                if (!Physics.Raycast(transform.position, transform.forward, distance))
                {
                    SpawnBlock(transform.position + transform.forward * distance);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (digAmount.Value <= 0) return;
                if (Physics.Raycast(transform.position, transform.forward, out var hit, distance))
                {
                    if (hit.transform.TryGetComponent(out NetworkObject networkObject))
                    {
                        DestroyBlockServerRpc(networkObject);
                    }
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
                blockCount.Value--;
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
                    blockCount.Value++;
                    digAmount.Value--;
                }
            }
        }
    }
}