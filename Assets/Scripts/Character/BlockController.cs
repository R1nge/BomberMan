using System;
using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class BlockController : NetworkBehaviour
    {
        [SerializeField] private float distance;
        [SerializeField] private NetworkVariable<int> digAmount;
        private MapGenerator _mapGenerator;
        private GameObject _block;
        private NetworkVariable<int> _blockCount;

        private void Awake()
        {
            _mapGenerator = FindObjectOfType<MapGenerator>();
            _blockCount = new NetworkVariable<int>();
        }

        public override void OnNetworkSpawn() => _block = _mapGenerator.GetCurrentMapConfig().obstacle;


        [ServerRpc]
        public void IncreaseDigAmountServerRpc(int amount) => digAmount.Value += amount;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_blockCount.Value <= 0) return;
                if (!Physics.Raycast(transform.position, transform.forward, distance))
                {
                    if (IsOwner)
                    {
                        SpawnBlockServerRpc(transform.position + transform.forward * distance);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (digAmount.Value <= 0) return;
                if (Physics.Raycast(transform.position, transform.forward, out var hit, distance))
                {
                    if (IsOwner)
                    {
                        if (hit.transform.TryGetComponent(out NetworkObject networkObject))
                        {
                            DestroyBlockServerRpc(networkObject);
                        }
                    }
                }
            }
        }

        [ServerRpc]
        private void SpawnBlockServerRpc(Vector3 pos)
        {
            var inst = Instantiate(_block, pos, Quaternion.identity);
            var netInst = inst.GetComponent<NetworkObject>();
            netInst.Spawn(true);
            netInst.GetComponent<PlaceInGrid>().PlaceInGridServerRpc();
            _blockCount.Value--;
        }

        [ServerRpc]
        private void DestroyBlockServerRpc(NetworkObjectReference obj)
        {
            if (obj.TryGet(out NetworkObject networkObject))
            {
                if (networkObject.TryGetComponent(out Obstacle obstacle))
                {
                    obstacle.TakeDamage(1000);
                    _blockCount.Value++;
                    digAmount.Value--;
                }
            }
        }
    }
}