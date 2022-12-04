using System;
using UnityEngine;
using UnityEngine.UI;

public class SkinUI : MonoBehaviour
{
    [SerializeField] private RawImage icon;
    [SerializeField] private int index;
    private PlayerSkins _skins;

    private void Awake() => _skins = FindObjectOfType<PlayerSkins>();

    private void Start() => LoadSkinImage();

    private void LoadSkinImage() => icon.texture = _skins.GetSprite(index).texture;
}