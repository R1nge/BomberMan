using Unity.Netcode;
using UnityEngine;

public class BombDistance : NetworkBehaviour
{
    [SerializeField] private int increaseTime;
    [SerializeField] private NetworkVariable<int> distance;
    private BombDistanceUI _bombDistanceUI;
    private GameState _gameState;
    private Timer _timer;

    public NetworkVariable<int> Distance => distance;

    private void Awake()
    {
        _bombDistanceUI = GetComponent<BombDistanceUI>();
        _gameState = FindObjectOfType<GameState>();
        _timer = FindObjectOfType<Timer>();
    }

    public override void OnNetworkSpawn() => distance.Value = 1;

    private void Start()
    {
        _timer.CurrentTime.OnValueChanged += OnTimePassed;
        distance.OnValueChanged += UpdateUI;
    }

    private void OnTimePassed(int previousvalue, int newvalue)
    {
        if (!_gameState.GameStarted.Value) return;
        if (_gameState.GameEnded.Value) return;
        if (newvalue % increaseTime == 0)
        {
            if (IsServer)
            {
                distance.Value += 1;
            }
        }
    }

    private void UpdateUI(int previousvalue, int newvalue) => _bombDistanceUI.UpdateUI(newvalue);
}