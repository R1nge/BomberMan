using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerNick : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI nick;
    private NetworkVariable<NetworkString> _nick = new NetworkVariable<NetworkString>();
    private Camera _camera;

    private void Awake() => _camera = Camera.main;

    public override void OnNetworkSpawn() => SetNickServerRpc();

    [ServerRpc(RequireOwnership = false)]
    private void SetNickServerRpc()
    {
        _nick.Value = PlayerPrefs.GetString("Nickname");
        SetNickClientRpc();
    }

    [ClientRpc]
    private void SetNickClientRpc() => nick.text = _nick.Value;

    private void Update()
    {
        nick.transform.LookAt(-_camera.transform.position);
    }
}