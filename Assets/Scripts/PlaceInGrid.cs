﻿using Unity.Netcode;
using UnityEngine;

public class PlaceInGrid : NetworkBehaviour
{
    [SerializeField] private float yOffset;
    private MapGenerator _mapGenerator;

    private void Awake() => _mapGenerator = FindObjectOfType<MapGenerator>();

    [ServerRpc]
    public void PlaceInGridServerRpc()
    {
        var position = transform.position;
        position = new Vector3(
            RoundToNearestGrid(position.x),
            position.y + yOffset,
            RoundToNearestGrid(position.z));
        transform.position = position;
    }

    private float RoundToNearestGrid(float pos)
    {
        var gridSize = _mapGenerator.GetCurrentMapConfig().tileSize;
        float xDiff = pos % gridSize;
        pos -= xDiff;
        if (xDiff > gridSize / 2)
        {
            pos += gridSize;
        }

        return pos;
    }
}