using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class LocalIpUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI localIp;

    private void Start()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIp.text = ip.ToString();
            }
        }
    }
}