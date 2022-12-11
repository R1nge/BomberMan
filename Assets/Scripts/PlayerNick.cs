using BayatGames.SaveGameFree;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerNick : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI nick;
    private NetworkVariable<NetworkString> _str;
    private Camera _camera;
    private string _nickStr;

    //TODO: cache Camera.Main

    private void Awake()
    {
        _str = new NetworkVariable<NetworkString>();
        _nickStr = SaveGame.Load<string>("Nickname") + Random.Range(0, 100);
    }

    private void Start() => NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

    private void OnClientConnected(ulong obj)
    {
        if (IsOwner)
        {
            SetNickServerRpc(_nickStr);
        }
        else
        {
            SetNickClientRpc(_nickStr);
        }
    }

    public override void OnNetworkSpawn()
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
        nick.transform.rotation = Camera.main.transform.rotation;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }
}