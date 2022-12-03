using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    public void UpdateUI(int value)
    {
        int minutes = Mathf.FloorToInt(value / 60f);
        int seconds = Mathf.FloorToInt(value - minutes * 60);
        string niceTime = $"{minutes:0}:{seconds:00}";
        textMeshProUGUI.text = niceTime;
    }
}