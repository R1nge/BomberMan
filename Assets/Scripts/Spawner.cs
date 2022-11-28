using System;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] private GameObject prefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        var inst = Instantiate(prefab);
        inst.GetComponent<NetworkObject>().Spawn();
    }
}