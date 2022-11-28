using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private NetworkVariable<int> health;

        public void TakeDamage(int amount)
        {
            health.Value -= amount;
            if (health.Value <= 0)
            {
                //Gameover
                Debug.LogWarning("Player died", this);
            }
        }
    }
}