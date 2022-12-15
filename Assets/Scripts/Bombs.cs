using BayatGames.SaveGameFree;
using Unity.Netcode;
using UnityEngine;

public class Bombs : NetworkBehaviour
{
    [SerializeField] private GameObject[] bombs;
    [SerializeField] private GameObject[] clientBombs;

    public GameObject GetBomb(int index) => bombs[index];

    public GameObject GetClientBomb(int index) => clientBombs[index];

    public void SetBomb(int index) => SaveGame.Save("Bomb", index);
}