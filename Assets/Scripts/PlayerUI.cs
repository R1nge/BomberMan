using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private TextMeshProUGUI bombs;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Destroy(UI);
            Destroy(this);
        }
    }
    
    public void UpdateBombs(int current, int max)
    {
        bombs.text = "Bombs: " + current + "/" + max;
    }
}