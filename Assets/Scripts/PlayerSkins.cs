using BayatGames.SaveGameFree;
using Unity.Netcode;
using UnityEngine;

public class PlayerSkins : NetworkBehaviour
{
    [SerializeField] private GameObject[] skins;

    public GameObject GetSkin(int index) => skins[index];

    public void SetSkin(int index) => SaveGame.Save("Skin", index);
}