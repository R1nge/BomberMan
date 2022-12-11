using TMPro;
using UnityEngine;

public class BombDistanceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notify;

    public void UpdateUI(int value) => notify.text = "Bomb distance: " + value;
}