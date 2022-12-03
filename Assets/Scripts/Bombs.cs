using BayatGames.SaveGameFree;
using Unity.Netcode;
using UnityEngine;

public class Bombs : NetworkBehaviour
{
    [SerializeField] private GameObject[] bombs;

    public GameObject GetBomb(int index) => bombs[index];

    public void SetBomb(int index) => SaveGame.Save("Bomb", index);
}