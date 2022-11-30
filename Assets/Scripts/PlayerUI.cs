using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private TextMeshProUGUI health, bombs;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Destroy(UI);
            Destroy(this);
        }
    }


    public void UpdateHealth(int old,int current)
    {
        health.text = "Health: " + current;
    }

    public void UpdateBombs(int old, int current)
    {
        bombs.text = "Bombs: " + current;
    }
}