using Unity.Netcode;
using UnityEngine;

namespace Powerups
{
    public abstract class Powerup : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent(out NetworkObject networkObject))
                {
                    PickUpServerRpc(networkObject.NetworkObjectId);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PickUpServerRpc(ulong ID)
        {
            var player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[ID];
            Apply(player);
            DestroyServerRpc();
        }

        protected virtual void Apply(NetworkObjectReference reference)
        {
        }

        [ServerRpc(RequireOwnership = false)]
        private void DestroyServerRpc() => GetComponent<NetworkObject>().Despawn();
    }
}