using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CountDown : NetworkBehaviour
{
    [SerializeField] private int timer;
    private NetworkVariable<int> _timer;
    private GameState _gameState;

    public event Action<int> OnTimeChanged;

    private void Awake()
    {
        _gameState = FindObjectOfType<GameState>();
        _timer = new NetworkVariable<int>(timer);
        _timer.OnValueChanged += (_, newValue) => { OnTimeChanged?.Invoke(newValue); };
        OnTimeChanged?.Invoke(_timer.Value);
    }

    private void Start()
    {
        if (!IsServer) return;
        StartCoroutine(Count());
    }

    private IEnumerator Count()
    {
        yield return new WaitForSeconds(1f);
        while (_timer.Value > 0)
        {
            yield return new WaitForSeconds(1f);
            _timer.Value--;
        }

        _gameState.StartGameServerRpc();
    }
}