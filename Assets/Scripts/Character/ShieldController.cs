﻿using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class ShieldController : NetworkBehaviour
    {
        [SerializeField] private GameObject shieldEffect;
        [SerializeField] private NetworkVariable<float> duration;
        private GameObject _shieldEffectRef;
        private NetworkVariable<bool> _isActive;

        public NetworkVariable<bool> IsActive => _isActive;

        private void Awake() => _isActive = new NetworkVariable<bool>();

        [ServerRpc(RequireOwnership = false)]
        public void ApplyShieldServerRpc()
        {
            if (_isActive.Value) return;
            _isActive.Value = true;
            SpawnShieldEffectClientRpc();
            StartCoroutine(Timer_c());
        }

        [ClientRpc]
        private void SpawnShieldEffectClientRpc()
        {
            _shieldEffectRef = Instantiate(shieldEffect.gameObject);
            _shieldEffectRef.transform.position = transform.position;
            _shieldEffectRef.transform.parent = transform;
        }

        [ServerRpc(RequireOwnership = false)]
        public void UseShieldServerRpc()
        {
            if (!_isActive.Value) return;
            UseShieldClientRpc();
            _isActive.Value = false;
        }

        [ClientRpc]
        private void UseShieldClientRpc() => Destroy(_shieldEffectRef);

        private IEnumerator Timer_c()
        {
            while (duration.Value > 0 && _isActive.Value)
            {
                yield return new WaitForSeconds(1);
                duration.Value -= 1;
                if (duration.Value <= 0)
                {
                    UseShieldServerRpc();
                }
            }
        }
    }
}