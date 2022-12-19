using BayatGames.SaveGameFree;
using Unity.Netcode;
using UnityEngine;

public class Bombs : NetworkBehaviour
{
    [SerializeField] private GameObject[] bombs;
    [SerializeField] private NetworkObject bombLogic;

    public GameObject GetBomb(int index) => bombs[index];

    public NetworkObject GetBombLogic() => bombLogic;

    public void SetBomb(int index) => SaveGame.Save("Bomb", index);
}