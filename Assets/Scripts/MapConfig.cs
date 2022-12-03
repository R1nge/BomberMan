using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig", menuName = "MapConfig")]
public class MapConfig : ScriptableObject
{
    public int minWidth, minHeight;
    public int maxWidth, maxHeight;
    public GameObject tile, destructable, borderWall, wall;
    public Vector3 tileOffset, destructableOffset, borderWallOffset, wallOffset;
    public float tileSize, destructableSize, borderWallSize, wallSize;

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