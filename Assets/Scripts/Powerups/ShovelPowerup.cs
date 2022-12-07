using Character;
using Unity.Netcode;
using UnityEngine;

namespace Powerups
{
    public class ShovelPowerup : Powerup
    {
        [SerializeField] private int amount;

        protected override void Apply(NetworkObjectReference reference)
        {
            if (reference.TryGet(out NetworkObject net))
            {
                if (net.TryGetComponent(out BlockController blockController))
                {
                    blockController.IncreaseDigAmountServerRpc(amount);
                }
            }
        }
    }
}