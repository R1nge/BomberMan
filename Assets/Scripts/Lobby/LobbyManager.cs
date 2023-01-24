using Lobby;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    private LobbyUI _lobbyUI;

    public void ReadyUp()
    {
        if (!IsServer)
        {
            ReadyUpServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReadyUpServerRpc(ServerRpcParams rpcParams = default)
    {
        for (var i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
        {
            if (LobbySingleton.Instance.GetPlayersList()[i].ClientId == rpcParams.Receive.SenderClientId)
            {
                LobbySingleton.Instance.GetPlayersList()[i] = new PlayerState(
                    LobbySingleton.Instance.GetPlayersList()[i].ClientId,
                    LobbySingleton.Instance.GetPlayersList()[i].SkinIndex,
                    !LobbySingleton.Instance.GetPlayersList()[i].IsReady,
                    LobbySingleton.Instance.GetPlayersList()[i].Nickname
                );

                _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
            }
        }

        OnLobbyPlayersStateChanged();
        PrintData();
    }

    private void PrintData()
    {
        for (int i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
        {
            print("ID: " + LobbySingleton.Instance.GetPlayersList()[i].ClientId);
            print("Is Ready: " + LobbySingleton.Instance.GetPlayersList()[i].IsReady);
            print("Skin Index: " + LobbySingleton.Instance.GetPlayersList()[i].SkinIndex);
            print("Nickname: " + LobbySingleton.Instance.GetPlayersList()[i].Nickname);
        }
    }

    private void Awake()
    {
        _lobbyUI = GetComponent<LobbyUI>();
        LobbySingleton.Instance.ResetPlayerList();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientConnected(ulong ID)
    {
        if (IsServer)
        {
            if (ID == 0)
            {
                LobbySingleton.Instance.GetPlayersList().Add(new PlayerState
                {
                    ClientId = ID,
                    SkinIndex = PlayerPrefs.GetInt("Skin"),
                    IsReady = true,
                    Nickname = PlayerPrefs.GetString("Nickname")
                });
            }


            for (int i = 0; i < 4; i++)
            {
                if (LobbySingleton.Instance.GetPlayersList().Count > i)
                {
                    _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
                    _lobbyUI.UpdateNicknameServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].Nickname);
                    _lobbyUI.UpdateSkin(i, LobbySingleton.Instance.GetPlayersList()[i].SkinIndex);
                    OnLobbyPlayersStateChanged();
                }
            }


            PrintData();
        }
        else
        {
            var skinIndex = PlayerPrefs.GetInt("Skin");
            var nickname = PlayerPrefs.GetString("Nickname");
            OnClientConnectedServerRpc(skinIndex, nickname);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnClientConnectedServerRpc(int skinIndex, NetworkString nickname, ServerRpcParams rpcParams = default)
    {
        print(LobbySingleton.Instance);
        print(LobbySingleton.Instance.GetPlayersList());
        LobbySingleton.Instance.GetPlayersList().Add(new PlayerState
        {
            ClientId = rpcParams.Receive.SenderClientId,
            SkinIndex = skinIndex,
            IsReady = false,
            Nickname = nickname
        });

        for (int i = 0; i < 4; i++)
        {
            if (LobbySingleton.Instance.GetPlayersList().Count > i)
            {
                _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
                _lobbyUI.UpdateNicknameServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].Nickname);
                _lobbyUI.UpdateSkin(i, LobbySingleton.Instance.GetPlayersList()[i].SkinIndex);
                OnLobbyPlayersStateChanged();
            }
        }
    }

    private void OnClientDisconnected(ulong ID)
    {
        if (!IsServer) return;
        for (var i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
        {
            if (LobbySingleton.Instance.GetPlayersList()[i].ClientId == ID)
            {
                LobbySingleton.Instance.GetPlayersList().Remove(LobbySingleton.Instance.GetPlayersList()[i]);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (LobbySingleton.Instance.GetPlayersList().Count > i)
            {
                _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
                _lobbyUI.UpdateSkin(i, LobbySingleton.Instance.GetPlayersList()[i].SkinIndex);
                OnLobbyPlayersStateChanged();
            }
            else
            {
                _lobbyUI.HideSkinServerRpc(i);
                _lobbyUI.ClearUIServerRpc(i);
            }
        }

        PrintData();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                OnClientConnected(client.ClientId);
            }

            _lobbyUI.UpdateStartButtonServerRpc(IsEveryoneReady());
        }

        for (int i = 0; i < 4; i++)
        {
            if (LobbySingleton.Instance.GetPlayersList().Count > i)
            {
                _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
                OnLobbyPlayersStateChanged();
            }
        }
    }

    private void OnLobbyPlayersStateChanged()
    {
        for (var i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
        {
            _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
        }

        _lobbyUI.UpdateStartButtonServerRpc(IsEveryoneReady());
    }

    public void StartGame()
    {
        if (!IsServer) return;
        if (IsEveryoneReady())
        {
            print("Started a game");
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }

    private bool IsEveryoneReady()
    {
        if (LobbySingleton.Instance.GetPlayersList().Count < 2)
        {
            return false;
        }

        foreach (var player in LobbySingleton.Instance.GetPlayersList())
        {
            if (!player.IsReady)
            {
                return false;
            }
        }

        return true;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}