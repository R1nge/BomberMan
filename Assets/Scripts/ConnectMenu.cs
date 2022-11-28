using TMPro;
using UnityEngine;

public class ConnectMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField nick;

    public void SaveNick()
    {
        PlayerPrefs.SetString("Nickname", nick.text);
        PlayerPrefs.Save();
    }
}