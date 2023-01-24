using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUI : NetworkBehaviour
    {
        [SerializeField] private LobbyCharacter[] characters;
        [SerializeField] private Button startGame, readyup;

        public override void OnNetworkSpawn()
        {
            startGame.gameObject.SetActive(IsServer);
            readyup.gameObject.SetActive(!IsServer);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void UpdateNicknameServerRpc(int index, string s)
        {
            characters[index].UpdateNicknameServerRpc(s);
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateReadyStateServerRpc(int index, bool state)
        {
            characters[index].UpdateReadyStateServerRpc(state);
        }


        public void UpdateSkin(int index, int skinIndex)
        {
            characters[index].UpdateSkinServerRpc(skinIndex);
        }

        [ServerRpc(RequireOwnership = false)]
        public void HideSkinServerRpc(int index)
        {
            characters[index].HideSkinServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateStartButtonServerRpc(bool state)
        {
            if (IsServer)
            {
                startGame.interactable = state;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ClearUIServerRpc(int i)
        {
            characters[i].ClearUIServerRpc();
        }
    }
}