using System;
using Character;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Bomb : NetworkBehaviour, IDamageable
{
    [SerializeField] private float gridSize;
    [SerializeField] private float explodeDelay;
    [SerializeField] private NetworkVariable<int> damage;
    [SerializeField] private float radius;
    [SerializeField] private Collider trigger;
    [SerializeField] private GameObject explosionSound;
    [SerializeField] private LayerMask ignore;
    private NetworkVariable<bool> _hasExploded;
    private NetworkVariable<float> _time;
    private BombDistance _bombDistance;

    public event Action<Bomb> OnBombExploded;

    private void Awake()
    {
        _hasExploded = new NetworkVariable<bool>();
        _time = new NetworkVariable<float>();
        _bombDistance = FindObjectOfType<BombDistance>();
        NetworkManager.NetworkTickSystem.Tick += OnTick;
    }

    private void Start() => Invoke(nameof(Explode), explodeDelay);

    private void OnTick()
    {
        if (IsServer)
        {
            if (_time.Value < explodeDelay)
            {
                _time.Value += NetworkManager.Singleton.LocalTime.FixedDeltaTime;
            }
        }
    }

    private void Explode()
    {
        if (_hasExploded.Value) return;
        OnBombExploded?.Invoke(this);
        var position = transform.position;
        Raycast(position, Vector3.forward, _bombDistance.Distance.Value, radius);
        Raycast(position, Vector3.back, _bombDistance.Distance.Value, radius);
        Raycast(position, Vector3.right, _bombDistance.Distance.Value, radius);
        Raycast(position, Vector3.left, _bombDistance.Distance.Value, radius);
        SpawnSoundServerRpc();
        DoDamageInside();
        Destroy();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnSoundServerRpc() => SpawnSoundClientRpc();

    [ClientRpc]
    private void SpawnSoundClientRpc() => Instantiate(explosionSound, transform.position, quaternion.identity);

    private void Raycast(Vector3 pos, Vector3 dir, int dist, float rad)
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

                    if (!obj.IsSpawned) return;
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
            var amount = _bombDistance.Distance.Value + 1;
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
        var vfx = VfxPool.Instance.GetPooledVfx();
        vfx.transform.position = transform.position + dir * i * gridSize;
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
        _hasExploded.Value = true;
        var obj = GetNetworkObject(ID);
        if (obj.TryGetComponent(out IDamageable damageable))
        {
            if (obj.CompareTag("Player"))
            {
                if (obj.TryGetComponent(out Health health))
                {
                    health.TakeDamagePlayer(damage, OwnerClientId, obj.OwnerClientId);
                }
            }
            else
            {
                damageable.TakeDamage(damage);
            }
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
                    if (obj == null || !obj.IsSpawned || obj == GetComponent<NetworkObject>()) return;
                    DoDamage(damage.Value, obj);
                }
            }
        }
    }

    private void Destroy()
    {
        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
        else
        {
            DestroyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyServerRpc() => GetComponent<NetworkObject>().Despawn();

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        trigger.isTrigger = false;
    }

    public void TakeDamage(int amount)
    {
        if (_hasExploded.Value) return;
        Explode();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.NetworkTickSystem.Tick -= OnTick;
    }
}