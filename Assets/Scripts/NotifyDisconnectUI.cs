using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NotifyDisconnectUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI disconnect;

    private void Awake()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong obj)
    {
        var client = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(obj);
        var nick = client.transform.Find("Canvas/Nick").GetComponent<TextMeshProUGUI>().text;
        OnClientDisconnectedClientRpc(nick);
    }

    [ClientRpc]
    private void OnClientDisconnectedClientRpc(NetworkString nick)
    {
        var str = nick + " has disconnected";
        disconnect.text = str;
        StartCoroutine(Clear_c());
    }

    private IEnumerator Clear_c()
    {
        yield return new WaitForSeconds(3);
        disconnect.text = String.Empty;
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
    }
}