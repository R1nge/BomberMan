using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI winner;

    [ServerRpc]
    public void WinServerRpc(string nickname)
    {
        GameoverClientRpc(nickname);
        StartCoroutine(Restart_c());
    }

    [ServerRpc]
    public void GameoverServerRpc()
    {
        GameoverClientRpc("TIE");
        StartCoroutine(Restart_c());
    }

    [ClientRpc]
    private void GameoverClientRpc(string text) => winner.text = text;

    private IEnumerator Restart_c()
    {
        yield return new WaitForSeconds(3);
        if (!IsServer) yield break;
        RestartServerRpc();
    }

    [ServerRpc]
    private void RestartServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}