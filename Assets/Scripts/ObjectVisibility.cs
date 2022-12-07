using Unity.Netcode;
using UnityEngine;

public class ObjectVisibility : NetworkBehaviour
{
    //TODO: fix
    [SerializeField] private float maxDistance;
    private NetworkObject _this;
    private ulong _playerID;

    private void Awake()
    {
        _this = GetComponent<NetworkObject>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += obj => { CheckServerRpc(); };
            NetworkManager.Singleton.SceneManager.OnLoadComplete += delegate { CheckServerRpc(); };
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void CheckServerRpc(ServerRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().IsOwner) return;
        if (Vector3.Distance(
            NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(rpcParams.Receive.SenderClientId).transform
                .position,
            transform.position) < maxDistance)
        {
            ShowServerRpc();
        }
        else
        {
            HideServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShowServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!_this.IsNetworkVisibleTo(rpcParams.Receive.SenderClientId))
        {
            _this.NetworkShow(rpcParams.Receive.SenderClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HideServerRpc(ServerRpcParams rpcParams = default)
    {
        if (_this.IsNetworkVisibleTo(rpcParams.Receive.SenderClientId))
        {
            _this.NetworkHide(rpcParams.Receive.SenderClientId);
        }
    }

    private void Update()
    {
        CheckServerRpc();
    }

    //TODO: 
    //get connected player ID
    //Save it
    //Pass it into visibility check
    //???
    //Profit
}