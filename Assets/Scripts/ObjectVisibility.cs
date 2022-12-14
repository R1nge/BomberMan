using Unity.Netcode;
using UnityEngine;

public class ObjectVisibility : NetworkBehaviour
{
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
        if (Vector3.Distance(
                player.transform
                    .position,
                transform.position) < maxDistance)
        {
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
    {
        if (client != NetworkManager.LocalClientId) return;
        if (_meshRenderer.enabled)
        {
            _meshRenderer.enabled = false;
        }
    }

    private void Update()
    {
        Check();
    }
}