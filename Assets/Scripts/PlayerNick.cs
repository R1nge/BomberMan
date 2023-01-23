using Lobby;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerNick : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI nick;
    private Camera _camera;
    private string _nickStr;

    //TODO: cache Camera.Main

    private void Awake()
    {
        if (!IsOwner) return;
        GetNicknameServerRpc();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    [ServerRpc]
    private void GetNicknameServerRpc()
    {
        var players = LobbySingleton.Instance.GetPlayersList();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == NetworkManager.Singleton.LocalClientId)
            {
                _nickStr = players[i].Nickname;
            }
        }
    }

    private void OnClientConnected(ulong obj)
    {
        if (!IsOwner) return;
        SetNickServerRpc();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        SetNickServerRpc();
    }

    [ServerRpc]
    private void SetNickServerRpc(ServerRpcParams rpcParams = default)
    {
        var players = LobbySingleton.Instance.GetPlayersList();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == rpcParams.Receive.SenderClientId)
            {
                _nickStr = players[i].Nickname;
                nick.text = _nickStr;
                SetNickClientRpc(_nickStr);
            }
        }
    }

    [ClientRpc]
    private void SetNickClientRpc(NetworkString str)
    {
        nick.text = str;
    }

    private void Update()
    {
        nick.transform.rotation = Camera.main.transform.rotation;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}