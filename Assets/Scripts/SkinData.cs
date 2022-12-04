using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "SkinData")]
public class SkinData : ScriptableObject
{
    public GameObject inGamePrefab;
    public GameObject previewPrefab;
    public Sprite iconPreview;
}