using System;
using System.Text;
using BayatGames.SaveGameFree;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectMenu : NetworkBehaviour
{
    [SerializeField] private TMP_InputField nickInput, ipInput, passwordInput;
    [SerializeField] private Button host, client;

    private void Awake()
    {
        ipInput.text = SaveGame.Load("LastIP", String.Empty);
        nickInput.text = SaveGame.Load("Nickname", nickInput.text);
    }

    private void Start()
    {
        host.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.ConnectionApprovalCallback ??= ApprovalCheck;
            NetworkManager.Singleton.StartHost();
        });

        client.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(passwordInput.text);
            NetworkManager.Singleton.StartClient();
        });

        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ipInput.text;
    }

    private void OnServerStarted() => LoadLobby();

    private void LoadLobby()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void SaveNick() => SaveGame.Save("Nickname", nickInput.text);

    public void SetIp()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ipInput.text;
        SaveGame.Save("LastIP", ipInput.text);
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        var connectionData = request.Payload;
        var password = Encoding.ASCII.GetString(connectionData);

        response.Approved = password == passwordInput.text;
        response.CreatePlayerObject = true;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
    }
}