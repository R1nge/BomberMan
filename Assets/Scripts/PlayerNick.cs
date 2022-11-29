using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerNick : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI nick;
    private NetworkVariable<NetworkString> _nick = new NetworkVariable<NetworkString>();
    private Camera _camera;
    private string _nickStr;

    private void Awake()
    {
        _camera = Camera.main;
        _nickStr = PlayerPrefs.GetString("Nickname");
    }

    public override void OnNetworkSpawn() => SetNickServerRpc();

    [ServerRpc(RequireOwnership = false)]
    private void SetNickServerRpc()
    {
        _nick.Value = _nickStr;
        SetNickClientRpc();
    }

    [ClientRpc]
    private void SetNickClientRpc() => nick.text = _nick.Value;

    private void Update()
    {
        nick.transform.rotation = _camera.transform.rotation;
    }
}