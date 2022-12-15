using Powerups;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "MapConfig", menuName = "MapConfig")]
public class MapConfig : ScriptableObject
{
    public int minWidth, minHeight;
    public int maxWidth, maxHeight;
    public Vector3 tileRotation, obstacleRotation, wallRotation;
    public AssetReferenceGameObject playerWall;
    public AssetReferenceGameObject tile, obstacle, border, wall;
    public Vector3 tileOffset, obstacleOffset, borderOffset, wallOffset;
    public float tileSize, obstacleSize, borderSize, wallSize;
    public Powerup[] drops;
    public Vector3 topRotation, leftRotation;

    public Vector2 GetSize()
    {
        var width = GetRandomOddNumber(minWidth, maxWidth);
        var height = GetRandomOddNumber(minHeight, maxHeight);
        return new Vector2(width, height);
    }

    private int GetRandomOddNumber(int min, int max)
    {
        var num = Random.Range(min, max + 1);
        if (num % 2 == 0)
        {
            num++;
        }

        return num;
    }
}