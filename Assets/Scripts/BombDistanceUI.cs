using System;
using TMPro;
using UnityEngine;

public class BombDistanceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notify;

    public void UpdateUI(int value)
    {
        notify.text = "Bomb distance: " + value;
        Invoke(nameof(Clear), 2);
    }

    private void Clear() => notify.text = String.Empty;
}