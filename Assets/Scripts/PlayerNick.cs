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
        _nickStr = PlayerPrefs.GetString("Nickname");
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong obj)
    {
        if (!IsOwner) return;
        SetNickServerRpc(_nickStr);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        SetNickServerRpc(_nickStr);
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