using Character;
using Unity.Netcode;
using UnityEngine;

namespace Powerups
{
    public class SpeedPowerup : Powerup
    {
        [SerializeField] private NetworkVariable<float> speed;

        protected override void Apply(NetworkObjectReference reference)
        {
            if (reference.TryGet(out NetworkObject net))
            {
                if (net.TryGetComponent(out MovementController movementController))
                {
                    movementController.SetSpeedServerRpc(speed.Value);
                }
            }
        }
    }
}