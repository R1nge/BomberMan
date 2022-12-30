using UnityEngine;
using UnityEngine.UI;

public class BombSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button select;

    public void SetIcon(Sprite sprite) => icon.sprite = sprite;

    public void SetCallback(PlayerBombs skin, BombSoundPreview sound, int index)
    {
        select.onClick.AddListener(() =>
        {
            skin.SetBomb(index);
            sound.Preview(index);
        });
    }
}