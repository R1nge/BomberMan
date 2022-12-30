using UnityEngine;

public class BombSoundPreview : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    private PlayerBombs _bombs;

    private void Awake() => _bombs = FindObjectOfType<PlayerBombs>();

    public void Preview(int index)
    {
        source.clip = _bombs.GetSound(index);
        source.Play(0);
    }
}