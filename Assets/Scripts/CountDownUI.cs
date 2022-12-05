using System;
using TMPro;
using UnityEngine;

public class CountDownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdown;

    public void UpdateUI(int value) => countdown.text = value > 0 ? value.ToString() : String.Empty;

    public void Hide() => countdown.text = String.Empty;
}