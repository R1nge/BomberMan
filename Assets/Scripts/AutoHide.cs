using UnityEngine;

public class AutoHide : MonoBehaviour
{
    [SerializeField] private float delay;

    private void OnEnable() => Invoke(nameof(Hide), delay);

    private void Hide() => gameObject.SetActive(false);
}