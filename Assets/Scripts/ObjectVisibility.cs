using System;
using Unity.Netcode;
using UnityEngine;

public class ObjectVisibility : NetworkBehaviour
{
    //TODO: Just use gameobject.SetActive(false) in clientRpc
    //TODO: Or create a manager, that holds refs to players and all objects
    //TODO: Then show for each client
    [SerializeField] private float maxDistance;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
<<<<<<< HEAD
            NetworkManager.Singleton.OnClientConnectedCallback += obj => { CheckServerRpc(); };
        }
        else
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += delegate { CheckServerRpc(); };
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckServerRpc(ServerRpcParams rpcParams = default)
    {
<<<<<<< HEAD
        if (NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().IsOwner) return;
=======
            NetworkManager.Singleton.OnClientConnectedCallback += obj => { Check(); };
        }
        else
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += delegate { Check(); };
        }
    }

    private void Check()
    {
        var localId = NetworkManager.Singleton.LocalClientId;
        var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (!player.IsSpawned) return;
>>>>>>> 0572340 (Added object visibility)
=======
        var localId = rpcParams.Receive.SenderClientId;
        var player = NetworkManager.Singleton.ConnectedClients[localId].PlayerObject;
        if(player.IsOwnedByServer) return;
>>>>>>> 065287a (Kinda working)
        if (Vector3.Distance(
                player.transform
                    .position,
                transform.position) < maxDistance)
        {
<<<<<<< HEAD
<<<<<<< HEAD
            ShowServerRpc();
=======
            ShowClientRpc(localId);
>>>>>>> 065287a (Kinda working)
        }
        else
        {
            HideClientRpc(localId);
        }
    }

    [ClientRpc]
    private void ShowClientRpc(ulong client)
    {
        if (client != NetworkManager.LocalClientId) return;
            if (!_meshRenderer.enabled)
            {
                _meshRenderer.enabled = true;
                //NetworkObject.gameObject.SetActive(true);
            }
    }

<<<<<<< HEAD
    [ServerRpc(RequireOwnership = false)]
    private void HideServerRpc(ServerRpcParams rpcParams = default)
=======
            Show(localId);
        }
        else
        {
            Hide(localId);
        }
    }

    private void Show(ulong client)
    {
        if (client != NetworkManager.LocalClientId) return;
        if (!_meshRenderer.enabled)
        {
            _meshRenderer.enabled = true;
        }
    }

    private void Hide(ulong client)
>>>>>>> 0572340 (Added object visibility)
=======
    [ClientRpc]
    private void HideClientRpc(ulong client)
>>>>>>> 065287a (Kinda working)
    {
        if (client != NetworkManager.LocalClientId) return;
        if (_meshRenderer.enabled)
        {
<<<<<<< HEAD
<<<<<<< HEAD
            _this.NetworkHide(rpcParams.Receive.SenderClientId);
=======
            _meshRenderer.enabled = false;
>>>>>>> 0572340 (Added object visibility)
=======
            _meshRenderer.enabled = false;
            //NetworkObject.gameObject.SetActive(false);
>>>>>>> 065287a (Kinda working)
        }
    }

    private void Update()
    {
<<<<<<< HEAD
<<<<<<< HEAD
=======
        //if (IsServer) return;
>>>>>>> 065287a (Kinda working)
        CheckServerRpc();
=======
        Check();
>>>>>>> 0572340 (Added object visibility)
    }
}