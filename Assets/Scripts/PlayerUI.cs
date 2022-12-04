using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private TextMeshProUGUI hp, bombs;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Destroy(UI);
            Destroy(this);
        }
    }

    public void UpdateHealth(int old, int current) => hp.text = current.ToString();

    public void UpdateBombs(int current, int max) => bombs.text = current + "/" + max;
}