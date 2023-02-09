using System;
using TMPro;
using UnityEngine;

public class CountDownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdown;
    private CountDown _countDown;

    private void Awake()
    {
        _countDown = FindObjectOfType<CountDown>();
        _countDown.OnTimeChanged += UpdateUI;
    }

    private void UpdateUI(int value) => countdown.text = value > 0 ? value.ToString() : String.Empty;
    
    private void OnDestroy() => _countDown.OnTimeChanged -= UpdateUI;
}