using BayatGames.SaveGameFree;
using UnityEngine;

public class CharacterPreview : MonoBehaviour
{
    [SerializeField] private Transform position;
    private GameObject _current;
    private PlayerSkins _skins;

    private void Awake()
    {
        _skins = FindObjectOfType<PlayerSkins>();
        _skins.OnSkinChanged += OnSkinChanged;

        SpawnPreview(_skins.GetSkinData(SaveGame.Load("Skin", 0)));
    }

    private void OnSkinChanged(SkinData skin) => SpawnPreview(skin);

    private void SpawnPreview(SkinData skin)
    {
        if (_current != null || _current != skin.lobbyPreview)
        {
            Destroy(_current);
        }

        _current = Instantiate(skin.offlinePreview, position.position, Quaternion.Euler(new Vector3(0, 180)));
    }

    private void OnDestroy()
    {
        if (_skins == null) return;
        _skins.OnSkinChanged -= OnSkinChanged;
    }
}