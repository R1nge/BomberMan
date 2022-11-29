using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float delay;

    private void Awake() => Invoke(nameof(Destroy), delay);

    private void Destroy() => Destroy(gameObject);
}