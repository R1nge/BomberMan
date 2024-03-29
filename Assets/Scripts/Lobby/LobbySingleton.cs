using System.Collections.Generic;
using UnityEngine;

namespace Lobby
{
    public class LobbySingleton : MonoBehaviour
    {
        public static LobbySingleton Instance { get; private set; }

        [SerializeField] private List<PlayerState> _lobbyPlayers;

        private void Awake()
        {
            _lobbyPlayers = new List<PlayerState>();
            if (Instance != null)
            {
                throw new System.Exception("Multiple LobbySingleton defined!");
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        public void ResetPlayerList() => _lobbyPlayers = new List<PlayerState>();

        public List<PlayerState> GetPlayersList() => _lobbyPlayers;
    }
}