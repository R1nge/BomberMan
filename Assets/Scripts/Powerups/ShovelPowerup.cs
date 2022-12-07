using Character;
using Unity.Netcode;

namespace Powerups
{
    public class ShovelPowerup : Powerup
    {
        protected override void Apply(NetworkObjectReference reference)
        {
            if (reference.TryGet(out NetworkObject net))
            {
                if (net.TryGetComponent(out BlockController blockController))
                {
                    blockController.IncreaseDigAmountServerRpc();
                }
            }
        }
    }
}