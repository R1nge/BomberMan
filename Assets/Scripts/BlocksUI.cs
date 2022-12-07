using TMPro;
using UnityEngine;

public class BlocksUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dig, block;

    public void UpdateDig(int amount) => dig.text = amount.ToString();

    public void UpdateBlock(int amount) => block.text = amount.ToString();
}