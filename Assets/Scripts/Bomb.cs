using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Bomb : NetworkBehaviour
{
    //TODO: Animations
    [SerializeField] private float gridSize;
    [SerializeField] private float explodeDelay;
    [SerializeField] private NetworkVariable<int> damage;
    [SerializeField] private int distance, radius;
    [SerializeField] private Collider trigger;
    [SerializeField] private float yOffset;
    [SerializeField] private GameObject explosionVFX, explosionSound;
    [SerializeField] private LayerMask ignore;

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
        Raycast(position, Vector3.forward, distance, radius);
        Raycast(position, Vector3.back, distance, radius);
        Raycast(position, Vector3.right, distance, radius);
        Raycast(position, Vector3.left, distance, radius);
        SpawnSoundServerRpc();
        DoDamageInside();

        Destroy();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnSoundServerRpc() => SpawnSoundClientRpc();

    [ClientRpc]
    private void SpawnSoundClientRpc() => Instantiate(explosionSound, transform.position, quaternion.identity);

    private void Raycast(Vector3 pos, Vector3 dir, int dist, int rad)
    {
        Ray ray = new Ray(pos, dir);
        if (Physics.SphereCast(ray, rad, out var hit, dist * gridSize, ~ignore))
        {
            if (hit.transform.TryGetComponent(out NetworkObject obj))
            {
                if (hit.transform.TryGetComponent(out IDamageable _))
                {
                    var amount = Mathf.CeilToInt((hit.distance + gridSize) / gridSize);
                    SpawnExplosionVfx(dir, amount);

                    if (!obj.IsSpawned || obj == null) return;
                    DoDamage(damage.Value, obj);
                }
                else
                {
                    var amount = Mathf.RoundToInt((hit.distance + gridSize) / gridSize);
                    SpawnExplosionVfx(dir, amount);
                }
            }
            else
            {
                var amount = Mathf.RoundToInt((hit.distance + gridSize) / gridSize);
                SpawnExplosionVfx(dir, amount);
            }
        }
        else
        {
            var amount = distance + 1;
            SpawnExplosionVfx(dir, amount);
        }
    }

    private void SpawnExplosionVfx(Vector3 dir, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnExplosionVfxServerRpc(dir, i);
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
        if (GetNetworkObject(ID).TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
        }
    }

    private void DoDamageInside()
    {
        var coll = new Collider[4];
        var size = Physics.OverlapBoxNonAlloc(transform.position, transform.localScale / 4, coll,
            Quaternion.identity);

        for (int i = 0; i < size; i++)
        {
            if (coll[i].TryGetComponent(out IDamageable _))
            {
                if (coll[i].TryGetComponent(out NetworkObject obj))
                {
                    if (obj == null || !obj.IsSpawned) return;
                    DoDamage(damage.Value, obj);
                }
            }
        }
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

    private void OnTriggerExit(Collider other) => trigger.isTrigger = false;
}