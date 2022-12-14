using Unity.Netcode;
using UnityEngine;

public class AutoDestroyServer : NetworkBehaviour
{
    [SerializeField] private float delay;

    public override void OnNetworkSpawn() => Invoke(nameof(DestroyServerRpc), delay);

    [ServerRpc(RequireOwnership = false)]
    private void DestroyServerRpc() => NetworkObject.Despawn();
}