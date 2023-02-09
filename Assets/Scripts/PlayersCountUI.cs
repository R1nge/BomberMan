using TMPro;
using UnityEngine;

public class PlayersCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;
    private PlayerSpawner _playerSpawner;

    private void Awake()
    {
        _playerSpawner = FindObjectOfType<PlayerSpawner>();
        _playerSpawner.PlayersAmount.OnValueChanged += OnPlayersAmount;
    }

    private void OnPlayersAmount(int _, int newValue)
    {
        countText.text = "Players alive: " + newValue;
    }
}