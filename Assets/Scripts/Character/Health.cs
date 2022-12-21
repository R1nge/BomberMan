﻿using System;
using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class Health : NetworkBehaviour, IDamageable
    {
        [SerializeField] private NetworkVariable<int> health;
        private PlayerSpawner _playerSpawner;
        private PlayerUI _playerUI;
        private CharacterShield _characterShield;
        public event Action OnTakenDamage;

        private void Awake()
        {
            _playerSpawner = FindObjectOfType<PlayerSpawner>();
            _playerUI = GetComponent<PlayerUI>();
            _characterShield = GetComponent<CharacterShield>();
            health.OnValueChanged += _playerUI.UpdateHealth;
        }

        public override void OnNetworkSpawn() => _playerUI.UpdateHealth(health.Value, health.Value);

        public void TakeDamage(int amount)
        {
            health.Value -= amount;
            if (health.Value <= 0)
            {
                _playerSpawner.Despawn(GetComponent<NetworkObject>().OwnerClientId,
                    GetComponent<NetworkObject>().OwnerClientId);
            }
        }

        public void TakeDamagePlayer(int amount, ulong who, ulong whom)
        {
            OnTakenDamage?.Invoke();
            if (_characterShield.IsActive.Value)
            {
                _characterShield.UseShieldServerRpc();
                return;
            }

            health.Value -= amount;
            if (health.Value <= 0)
            {
                _playerSpawner.Despawn(who, whom);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseHealthServerRpc(int amount) => health.Value += amount;
    }
}