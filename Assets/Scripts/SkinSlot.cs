using UnityEngine;
using UnityEngine.UI;

public class SkinSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button select;

    public void SetIcon(Sprite sprite) => icon.sprite = sprite;

    public void SetCallback(PlayerSkins skin, int index) => select.onClick.AddListener(() => { skin.SetSkin(index); });
}