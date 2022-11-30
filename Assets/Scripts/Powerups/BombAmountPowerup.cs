using Character;
using Unity.Netcode;
using UnityEngine;

namespace Powerups
{
    public class BombAmountPowerup : Powerup
    {
        [SerializeField] private int amount;

        protected override void Apply(NetworkObjectReference reference)
        {
            if (reference.TryGet(out NetworkObject net))
            {
                if (net.TryGetComponent(out BombController bombController))
                {
                    bombController.IncreaseBombAmountServerRpc(amount);
                }
            }
        }
    }
}