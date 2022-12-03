using Character;
using Unity.Netcode;
using UnityEngine;

namespace Powerups
{
    public class HealthPowerup : Powerup
    {
        [SerializeField] private int amount;
        
        protected override void Apply(NetworkObjectReference reference)
        {
            if (reference.TryGet(out NetworkObject net))
            {
                if (net.TryGetComponent(out Health health))
                {
                    //health.IncreaseHealthServerRpc(amount);
                }
            }
        }
    }
}