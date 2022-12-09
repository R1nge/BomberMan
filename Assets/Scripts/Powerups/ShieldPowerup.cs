using Character;
using Unity.Netcode;

namespace Powerups
{
    public class ShieldPowerup : Powerup
    {
        protected override void Apply(NetworkObjectReference reference)
        {
            if (reference.TryGet(out NetworkObject net))
            {
                if (net.TryGetComponent(out ShieldController shieldController))
                {
                    shieldController.ApplyShieldServerRpc();
                }
            }
        }
    }
}