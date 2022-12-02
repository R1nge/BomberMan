using System;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject player;
    [SerializeField] private Transform[] positions;
    private NetworkVariable<int> _playersAmount = new NetworkVariable<int>();

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Check;
    }

    private void Check(ulong ID)
    {
        if (_playersAmount.Value >= 4)
        {
            NetworkManager.Singleton.DisconnectClient(ID);
            _playersAmount.Value = 4;
        }
    }

    public override void OnNetworkSpawn() => SpawnPreview();

    private void SpawnPreview()
    {
        if (IsServer)
        {
            var rot = Quaternion.Euler(new Vector3(0, 180, 0));
            Instantiate(player, positions[_playersAmount.Value].position, rot).Spawn();
            _playersAmount.Value++;
        }
        else
        {
            SpawnPreviewServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPreviewServerRpc() => SpawnPreview();

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= Check;
    }
}