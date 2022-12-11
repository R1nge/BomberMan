using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class KillFeed : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI killFeed;
    private NetworkVariable<NetworkString> _display;

    private void Awake() => _display = new NetworkVariable<NetworkString>();

    public override void OnNetworkSpawn() => _display.OnValueChanged += OnValueChanged;

    private void OnValueChanged(NetworkString previousvalue, NetworkString newvalue)
    {
        killFeed.text = newvalue;
    }

    [ServerRpc]
    public void DisplayKillServerRpc(NetworkString who, NetworkString whom)
    {
        _display.Value = who + " killed " + whom;
        StartCoroutine(ResetText_c());
    }

    private IEnumerator ResetText_c()
    {
        yield return new WaitForSeconds(3);
        _display.Value = string.Empty;
    }
}