using System.Collections;
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
    [SerializeField] private GameObject explosionVFX, explosionSound;
    [SerializeField] private LayerMask ignore;
    [SerializeField] private Color explosionColor;
    private NetworkVariable<bool> _hasExploded;
    private MeshRenderer _meshRenderer;
    private NetworkVariable<float> _time;
    private BombDistance _bombDistance;

    public float ExplodeDelay => explodeDelay;

    private void Awake()
    {
        _hasExploded = new NetworkVariable<bool>();
        _time = new NetworkVariable<float>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _bombDistance = FindObjectOfType<BombDistance>();
    }

    private void Start() => Invoke(nameof(Explode), explodeDelay);

    public override void OnNetworkSpawn()
    {
        _time.OnValueChanged +=
            (value, newValue) => UpdateColor(explosionColor, newValue / explodeDelay / 100);
    }

    //https://docs-multiplayer.unity3d.com/netcode/current/advanced-topics/networktime-ticks/#example-1-using-network-time-to-synchronize-environments
    private void Update()
    {
        if (IsServer)
        {
            if (_time.Value < explodeDelay)
            {
                _time.Value += Time.deltaTime;
            }
        }
    }

    private void UpdateColor(Color color, float lerp)
    {
        ChangeColorServerRpc(NetworkManager.LocalTime.Time, color, lerp);
        StartCoroutine(WaitSync(0, color, lerp));
    }

    private IEnumerator WaitSync(float timeToWait, Color color, float lerp)
    {
        if (timeToWait > 0)
        {
            yield return new WaitForSeconds(timeToWait);
        }

        _meshRenderer.material.color = Color.Lerp(_meshRenderer.materials[0].color, color, lerp);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeColorServerRpc(double time, Color color, float lerp)
    {
        ChangeColorClientRpc(time, color, lerp);
        var timeToWait = time - NetworkManager.ServerTime.Time;
        StartCoroutine(WaitSync((float) timeToWait, color, lerp));
    }

    [ClientRpc]
    private void ChangeColorClientRpc(double time, Color color, float lerp)
    {
        if (IsOwner) return;
        var timeToWait = time - NetworkManager.ServerTime.Time;
        StartCoroutine(WaitSync((float) timeToWait, color, lerp));
    }

    private void Explode()
    {
        if (_hasExploded.Value) return;
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
            var amount = _bombDistance.Distance.Value + 1;
            SpawnExplosionVfx(dir, amount);
        }
    }

    private void SpawnExplosionVfx(Vector3 dir, int amount)
    {
        for (int i = 0;
            i < amount;
            i++)
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
        _hasExploded.Value = true;
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
        for (int i = 0;
            i < size;
            i++)
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
}