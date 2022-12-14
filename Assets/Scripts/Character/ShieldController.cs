using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class ShieldController : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<float> duration;
        private NetworkVariable<bool> _isActive;

        public NetworkVariable<bool> IsActive => _isActive;

        private void Awake() => _isActive = new NetworkVariable<bool>();

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
            _isActive.Value = false;
        }

        private IEnumerator Timer_c()
        {
            while (duration.Value > 0 && _isActive.Value)
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
}