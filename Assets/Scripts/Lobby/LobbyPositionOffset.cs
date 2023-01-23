using Unity.Netcode;
using UnityEngine;

namespace Lobby
{
    public class LobbyPositionOffset : NetworkBehaviour
    {
        [SerializeField] private Vector3 offset;

        public override void OnNetworkSpawn()
        {
            transform.position += offset;
        }
    }
}