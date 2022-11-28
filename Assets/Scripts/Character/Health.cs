using UnityEngine;

namespace Character
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private int health;

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
            {
                //Gameover
                Debug.LogWarning("Player died", this);
            }
        }
    }
}