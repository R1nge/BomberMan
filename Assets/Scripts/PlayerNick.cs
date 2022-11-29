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
        _nick = new NetworkVariable<NetworkString>();
        _nickStr = Random.Range(0, 11).ToString();
        //The problem is in Player prefs or TMP save text
    }

    public override void OnNetworkSpawn() => SetNickServerRpc(_nickStr);

    [ServerRpc(RequireOwnership = false)]
    private void SetNickServerRpc(NetworkString str)
    {
        _nick.Value = str;
        SetNickClientRpc(str);
    }

    [ClientRpc]
    private void SetNickClientRpc(string str) => nick.text = str;

    private void Update()
    {
        nick.transform.rotation = _camera.transform.rotation;
    }
}