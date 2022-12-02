using BayatGames.SaveGameFree;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerNick : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI nick;
    private NetworkVariable<NetworkString> _str = new NetworkVariable<NetworkString>();
    private Camera _camera;
    private string _nickStr;

    private void Awake()
    {
        _camera = Camera.main;
        _nickStr = SaveGame.Load<string>("Nickname");
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong obj)
    {
        if (!IsOwner) return;
        SetNickServerRpc(_nickStr);
    }

    private void Start()
    {
        if (!IsOwner) return;
        SetNickServerRpc(_nickStr);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetNickServerRpc(NetworkString str)
    {
        _str.Value = str;
        nick.text = str;
        SetNickClientRpc(_str.Value);
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

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }
}