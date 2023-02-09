using TMPro;
using UnityEngine;

public class BombDistanceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notify;
    private BombDistance _bombDistance;

    private void Awake()
    {
        _bombDistance = FindObjectOfType<BombDistance>();
        _bombDistance.OnDistanceChanged += UpdateUI;
    }

    private void UpdateUI(int value) => notify.text = "Bomb distance: " + value;

    private void OnDestroy() => _bombDistance.OnDistanceChanged -= UpdateUI;
}