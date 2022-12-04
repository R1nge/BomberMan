using BayatGames.SaveGameFree;
using Unity.Netcode;
using UnityEngine;

public class PlayerSkins : NetworkBehaviour
{
    [SerializeField] private SkinData[] skins;

    public GameObject GetPlayerPrefab(int index) => skins[index].inGamePrefab;

    public GameObject GetPreviewPrefab(int index) => skins[index].previewPrefab;

    public Sprite GetSprite(int index) => skins[index].iconPreview;

    public void SetSkin(int index) => SaveGame.Save("Skin", index);
}