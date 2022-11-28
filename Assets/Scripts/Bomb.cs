using UnityEngine;

public class Bomb : MonoBehaviour //, IDamageable
{
    //Animations
    [SerializeField] private float explodeDelay;
    [SerializeField] private int damage;
    [SerializeField] private int distance;
    [SerializeField] private Collider trigger;

    private void Start() => Invoke(nameof(Explode), explodeDelay);

    private void Explode()
    {
        Raycast(Vector3.forward);
        Raycast(Vector3.back);
        Raycast(Vector3.right);
        Raycast(Vector3.left);
        Destroy(gameObject);
    }

    private void Raycast(Vector3 dir)
    {
        if (Physics.Raycast(transform.position, dir, out var hit, distance))
        {
            if (hit.transform.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
        }
    }

    //public void TakeDamage(int amount) => Explode();

    private void OnTriggerExit(Collider other) => trigger.isTrigger = false;
}