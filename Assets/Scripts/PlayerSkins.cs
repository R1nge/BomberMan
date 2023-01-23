using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerSkins : NetworkBehaviour
{
    [SerializeField] private SkinData[] skins;

    public event Action<SkinData> OnSkinChanged;

    public SkinData GetSkinData(int index) => skins[index];

    public int GetSkinsCount() => skins.Length;

    public GameObject GetPlayerPrefab(int index) => skins[index].inGamePrefab;

    public GameObject GetPreviewPrefab(int index) => skins[index].lobbyPreview;

    public Sprite GetSprite(int index) => skins[index].iconPreview;

    public void SetSkin(int index)
    {
        OnSkinChanged?.Invoke(skins[index]);
        PlayerPrefs.SetInt("Skin", index);
    }
}