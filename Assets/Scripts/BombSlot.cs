using UnityEngine;
using UnityEngine.UI;

public class BombSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button select;

    public void SetIcon(Sprite sprite) => icon.sprite = sprite;

    public void SetCallback(PlayerBombs bomb, BombSoundPreview sound, int index)
    {
        select.onClick.AddListener(() =>
        {
            bomb.SetBomb(index);
            sound.Preview(index);
        });
    }
}