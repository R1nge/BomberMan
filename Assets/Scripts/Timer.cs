using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Timer : NetworkBehaviour
{
    [SerializeField] private int startTime;
    private NetworkVariable<int> _currentTime;
    private TimerUI _timerUI;
    private GameState _gameState;

    public NetworkVariable<int> CurrentTime => _currentTime;

    private void Awake()
    {
        _currentTime = new NetworkVariable<int>();
        _timerUI = FindObjectOfType<TimerUI>();
        _gameState = FindObjectOfType<GameState>();
        _gameState.OnGameStarted += () => StartCoroutine(Timer_c());
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _currentTime.Value = startTime;
        _timerUI.UpdateUI(_currentTime.Value);
        _currentTime.OnValueChanged += UpdateUIClientRpc;
    }

    private IEnumerator Timer_c()
    {
        if(!IsServer) yield break;
        if (_currentTime.Value <= 0)
        {
            _gameState.GameOverServerRpc();
            yield break;
        }

        yield return new WaitForSeconds(1);
        _currentTime.Value--;
        StartCoroutine(Timer_c());
    }

    [ClientRpc]
    private void UpdateUIClientRpc(int old, int value) => _timerUI.UpdateUI(value);
}