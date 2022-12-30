using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectUI : MonoBehaviour
{
    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }
}