using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "SkinData")]
public class SkinData : ScriptableObject
{
    public GameObject inGamePrefab;
    public GameObject lobbyPreview;
    public GameObject offlinePreview;
    public Sprite iconPreview;
}