﻿using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private NetworkVariable<int> players;
    private SpawnPositions _spawnPositions;

    private void Awake()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            players = new NetworkVariable<int>();
        }

        _spawnPositions = FindObjectOfType<SpawnPositions>();
    }

    public override void OnNetworkSpawn()
    {
        StartCoroutine(Wait_C());
    }
    
    private IEnumerator Wait_C()
    {
        yield return new WaitForSeconds(1f);
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (IsServer)
        {
            //Quick and dirty hack, but i'll leave it for now
            var player = Instantiate(playerPrefab, _spawnPositions.GetPositions()[players.Value], Quaternion.identity);
            player.GetComponent<NetworkObject>().SpawnWithOwnership((ulong) players.Value);
            player.GetComponent<NetworkObject>().transform.position = _spawnPositions.GetPositions()[players.Value];
            players.Value++;
        }
        else
        {
            SpawnPlayerServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc() => SpawnPlayer();

    public override void OnDestroy()
    {
        base.OnDestroy();
        players.Dispose();
    }
}