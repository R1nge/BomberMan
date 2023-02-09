using System;
using Unity.Netcode;
using UnityEngine;

public class BombDistance : NetworkBehaviour
{
    [SerializeField] private int maxDistance;
    [SerializeField] private int increaseTime;
    [SerializeField] private NetworkVariable<int> distance;
    private GameState _gameState;
    private Timer _timer;

    public NetworkVariable<int> Distance => distance;

    public event Action<int> OnDistanceChanged; 
    
    private void Awake()
    {
        _gameState = FindObjectOfType<GameState>();
        _timer = FindObjectOfType<Timer>();
    }

    public override void OnNetworkSpawn() => distance.Value = 1;

    private void Start()
    {
        _timer.CurrentTime.OnValueChanged += OnTimePassed;
        distance.OnValueChanged += OnValueChanged;;
    }

    private void OnValueChanged(int _, int newValue) => OnDistanceChanged?.Invoke(newValue);

    private void OnTimePassed(int previousvalue, int newvalue)
    {
        if (!IsServer) return;
        if (distance.Value >= maxDistance) return;
        if (!_gameState.GameStarted.Value) return;
        if (_gameState.GameEnded.Value) return;
        if (newvalue % increaseTime == 0)
        {
            distance.Value += 1;
        }
    }
}