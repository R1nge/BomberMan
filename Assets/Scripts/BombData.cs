using UnityEngine;

[CreateAssetMenu(fileName = "BombData", menuName = "BombData")]
public class BombData : ScriptableObject
{
    public GameObject inGamePrefab;
    public Sprite iconPreview;
    public AudioClip sound;
}