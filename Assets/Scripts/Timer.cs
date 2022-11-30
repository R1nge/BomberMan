using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Timer : NetworkBehaviour
{
    [SerializeField] private int startTime;
    private NetworkVariable<int> _currentTime = new NetworkVariable<int>();
    private TimerUI _timerUI;
    private GameState _gameState;

    private void Awake()
    {
        _timerUI = FindObjectOfType<TimerUI>();
        _gameState = FindObjectOfType<GameState>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _currentTime.Value = startTime;
        _currentTime.OnValueChanged += UpdateUIClientRpc;
        StartCoroutine(Timer_c());
    }

    private IEnumerator Timer_c()
    {
        if (_currentTime.Value <= 0)
        {
            _gameState.GameoverServerRpc();
            yield break;
        }

        yield return new WaitForSeconds(1);
        _currentTime.Value--;
        StartCoroutine(Timer_c());
    }

    [ClientRpc]
    private void UpdateUIClientRpc(int old, int value)
    {
        _timerUI.UpdateUI(value);
    }
}