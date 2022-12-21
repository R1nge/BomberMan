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
                if (net.TryGetComponent(out CharacterBlock blockController))
                {
                    blockController.IncreaseDigAmountServerRpc();
                }
            }
        }
    }
}