using BayatGames.SaveGameFree;
using UnityEngine;

public class PlayerBombs : MonoBehaviour
{
    [SerializeField] private BombData[] bombs;

    public int GetBombsCount() => bombs.Length;

    public GameObject GetBombPrefab(int index) => bombs[index].inGamePrefab;

    public Sprite GetSprite(int index) => bombs[index].iconPreview;

    public AudioClip GetSound(int index) => bombs[index].sound;

    public void SetBomb(int index) => SaveGame.Save("Bomb", index);
}