using UnityEngine;

public class BombSoundPreview : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private AudioSource source;

    public void Preview(int index)
    {
        source.clip = sounds[index];
        source.Play(0);
    }
}