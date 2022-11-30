using BayatGames.SaveGameFree;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ConnectMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField nick, ip;

    public void SaveNick() => SaveGame.Save("Nickname", nick.text);

    public void SetIp()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ip.text;
    }
}