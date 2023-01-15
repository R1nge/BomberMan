using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class KillFeed : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI[] killFeed;
    private NetworkVariable<NetworkString> _display;

    private void Awake() => _display = new NetworkVariable<NetworkString>();

    public override void OnNetworkSpawn() => _display.OnValueChanged += OnValueChanged;

    [ServerRpc]
    public void DisplayKillServerRpc(NetworkString who, NetworkString whom)
    {
        if (who == whom)
        {
            _display.Value = who + " suicided";
        }
        else
        {
            _display.Value = who + " killed " + whom;
        }
    }

    private void OnValueChanged(NetworkString previousvalue, NetworkString newvalue)
    {
        print("Value changed");
        for (int i = 0; i < killFeed.Length; i++)
        {
            if (string.IsNullOrEmpty(killFeed[i].text))
            {
                killFeed[i].text = newvalue;
                print("Text changed");
                Invoke(nameof(ResetText), 3f);
                break;
            }
        }
    }

    private void ResetText()
    {
        for (int i = 0; i < killFeed.Length; i++)
        {
            if (!string.IsNullOrEmpty(killFeed[i].text))
            {
                killFeed[i].text = String.Empty;
                break;
            }
        }
    }
}