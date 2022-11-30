using System;
using BayatGames.SaveGameFree;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerNick : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI nick;
    private Camera _camera;
    private string _nickStr;

    private void Awake()
    {
        _camera = Camera.main;
        _nickStr = SaveGame.Load<string>("Nickname");
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            if (!IsOwner) return;
            SetNickServerRpc(_nickStr);
        }
        else
        {
            SetNickServerRpc(_nickStr);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetNickServerRpc(NetworkString str)
    {
        nick.text = str;
        SetNickClientRpc(str);
    }

    [ClientRpc]
    private void SetNickClientRpc(NetworkString str)
    {
        nick.text = str;
    }

    private void Update()
    {
        if (!IsOwner) return;
        nick.transform.rotation = _camera.transform.rotation;
    }
}