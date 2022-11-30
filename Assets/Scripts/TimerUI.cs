using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    public void UpdateUI(int value) => textMeshProUGUI.text = value.ToString();
}