using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Bomb : NetworkBehaviour //, IDamageable
{
    //Animations
    [SerializeField] private float gridSize;
    [SerializeField] private float explodeDelay;
    [SerializeField] private NetworkVariable<int> damage;
    [SerializeField] private int distance;
    [SerializeField] private Collider trigger;
    [SerializeField] private float yOffset;
    [SerializeField] private GameObject explosionVFX, explosionSound;

    public float ExplodeDelay => explodeDelay;

    private void Start() => Invoke(nameof(Explode), explodeDelay);

    public override void OnNetworkSpawn()
    {
        var position = transform.position;
        position = new Vector3(
            RoundToNearestGrid(position.x),
            position.y + yOffset,
            RoundToNearestGrid(position.z));
        transform.position = position;
    }

    //TODO: take grid length into a count 
    float RoundToNearestGrid(float pos)
    {
        float xDiff = pos % gridSize;
        pos -= xDiff;
        if (xDiff > gridSize / 2)
        {
            pos += gridSize;
        }

        return pos;
    }

    private void Explode()
    {
        var position = transform.position;
        Raycast(position, Vector3.forward, distance);
        Raycast(position, Vector3.back, distance);
        Raycast(position, Vector3.right, distance);
        Raycast(position, Vector3.left, distance);
        SpawnSoundServerRpc();
        Destroy();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnSoundServerRpc() => SpawnSoundClientRpc();

    [ClientRpc]
    private void SpawnSoundClientRpc() => Instantiate(explosionSound, transform.position, quaternion.identity);

    private void Raycast(Vector3 pos, Vector3 dir, int dist)
    {
        if (Physics.Raycast(pos, dir, out var hit, dist * gridSize))
        {
            if (hit.transform.TryGetComponent(out NetworkObject obj))
            {
                DoDamage(damage.Value, obj);
            }

            var amount = (hit.distance / gridSize) + 1;
            for (int i = 1; i < amount; i++)
            {
                SpawnExplosionVfxServerRpc(dir, i);
            }
        }
        else
        {
            var amount = distance + 1;
            for (int i = 1; i < amount; i++)
            {
                SpawnExplosionVfxServerRpc(dir, i);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnExplosionVfxServerRpc(Vector3 dir, int i) => SpawnExplosionVfxClientRpc(dir, i);

    [ClientRpc]
    private void SpawnExplosionVfxClientRpc(Vector3 dir, int i)
    {
        Instantiate(explosionVFX, transform.position + dir * i * gridSize, Quaternion.identity);
    }

    private void DoDamage(int damage, NetworkObjectReference hit)
    {
        if (hit.TryGet(out NetworkObject obj))
        {
            DoDamageServerRpc(damage, obj.NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DoDamageServerRpc(int damage, ulong ID)
    {
        GetNetworkObject(ID).GetComponent<IDamageable>().TakeDamage(damage);
    }

    private void Destroy()
    {
        if (!IsServer)
        {
            DestroyServerRpc();
        }
        else
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyServerRpc() => GetComponent<NetworkObject>().Despawn();


    //public void TakeDamage(int amount) => Explode();

    private void OnTriggerExit(Collider other) => trigger.isTrigger = false;
}