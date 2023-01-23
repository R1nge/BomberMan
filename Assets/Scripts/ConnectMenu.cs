using System;
using System.Text;
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
        ipInput.text = PlayerPrefs.GetString("LastIP", String.Empty);
        nickInput.text = PlayerPrefs.GetString("Nickname", nickInput.text);
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

    public void SaveNick()
    {
        PlayerPrefs.SetString("Nickname", nickInput.text);
        PlayerPrefs.Save();
    }

    public void SetIp()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ipInput.text;
        PlayerPrefs.SetString("LastIP", ipInput.text);
        PlayerPrefs.Save();
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