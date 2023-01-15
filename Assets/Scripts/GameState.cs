using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI winner;
    private NetworkVariable<bool> _gameStarted;
    private NetworkVariable<bool> _gameEnded;
    public event Action OnGameStarted;

    public NetworkVariable<bool> GameStarted => _gameStarted;
    public NetworkVariable<bool> GameEnded => _gameEnded;

    private void Awake()
    {
        _gameStarted = new NetworkVariable<bool>();
        _gameEnded = new NetworkVariable<bool>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc()
    {
        if (_gameEnded.Value) return;
        if (_gameStarted.Value) return;
        _gameStarted.Value = true;
        StartGameClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void TieServerRpc()
    {
        TieClientRpc();
        StartCoroutine(Restart_c());
    }

    [ClientRpc]
    private void TieClientRpc() => winner.text = "TIE";

    [ClientRpc]
    private void StartGameClientRpc() => OnGameStarted?.Invoke();

    [ServerRpc]
    public void WinServerRpc(string nickname)
    {
        if (_gameEnded.Value) return;
        _gameEnded.Value = true;
        GameoverClientRpc(nickname);
        StartCoroutine(Restart_c());
    }

    [ServerRpc(RequireOwnership = false)]
    public void GameOverServerRpc()
    {
        if (_gameEnded.Value) return;
        _gameEnded.Value = true;
        GameoverClientRpc("GAMEOVER");
        StartCoroutine(Restart_c());
    }

    [ClientRpc]
    private void GameoverClientRpc(string text) => winner.text = text;

    private IEnumerator Restart_c()
    {
        yield return new WaitForSeconds(3);
        if (!IsServer) yield break;
        RestartServerRpc();
    }

    [ServerRpc]
    private void RestartServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}