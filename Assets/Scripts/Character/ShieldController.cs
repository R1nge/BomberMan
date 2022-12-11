﻿using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class ShieldController : NetworkBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] private NetworkVariable<float> duration;
        [SerializeField] private int distance;
        [SerializeField] private float radius;
        [SerializeField] private GameObject explosionVFX;
        [SerializeField] private LayerMask ignore;
        private NetworkVariable<bool> _isActive;
        private MapGenerator _mapGenerator;

        public NetworkVariable<bool> IsActive => _isActive;

        private void Awake()
        {
            _isActive = new NetworkVariable<bool>();
            _mapGenerator = FindObjectOfType<MapGenerator>();
        }

        [ServerRpc(RequireOwnership = false)]
        public void ApplyShieldServerRpc()
        {
            if (_isActive.Value) return;
            _isActive.Value = true;
            StartCoroutine(Timer_c());
        }

        [ServerRpc(RequireOwnership = false)]
        public void UseShieldServerRpc()
        {
            if (!_isActive.Value) return;

            var position = transform.position;

            Raycast(position, Vector3.forward, distance, radius);
            Raycast(position, Vector3.back, distance, radius);
            Raycast(position, Vector3.right, distance, radius);
            Raycast(position, Vector3.left, distance, radius);
            _isActive.Value = false;
        }

        private void Raycast(Vector3 pos, Vector3 dir, int dist, float rad)
        {
            Ray ray = new Ray(pos, dir);
            var size = _mapGenerator.GetCurrentMapConfig().tileSize;
            if (Physics.SphereCast(ray, rad, out var hit, dist * size, ~ignore))
            {
                if (hit.transform.TryGetComponent(out NetworkObject obj))
                {
                    if (hit.transform.TryGetComponent(out IDamageable _))
                    {
                        var amount = Mathf.CeilToInt((hit.distance + size) / size);
                        SpawnExplosionVfx(dir, amount);

                        if (!obj.IsSpawned || obj == null) return;
                        DoDamage(damage, obj);
                    }
                    else
                    {
                        var amount = Mathf.RoundToInt((hit.distance + size) / size);
                        SpawnExplosionVfx(dir, amount);
                    }
                }
                else
                {
                    var amount = Mathf.RoundToInt((hit.distance + size) / size);
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
        private void SpawnExplosionVfxServerRpc(Vector3 dir, int i) =>
            SpawnExplosionVfxClientRpc(dir, i, _mapGenerator.GetCurrentMapConfig().tileSize);

        [ClientRpc]
        private void SpawnExplosionVfxClientRpc(Vector3 dir, int i, float size)
        {
            Instantiate(explosionVFX, transform.position + dir * i * size, Quaternion.identity);
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

        private IEnumerator Timer_c()
        {
            yield return new WaitForSeconds(1);
            duration.Value -= 1;
            if (duration.Value <= 0)
            {
                _isActive.Value = false;
            }
        }
    }
}