using UnityEngine;

public class BombColor : MonoBehaviour
{
    [SerializeField] private float explodeDelay;
    [SerializeField] private Color explosionColor;
    private MeshRenderer _meshRenderer;
    private float _time;

    private void Awake() => _meshRenderer = GetComponent<MeshRenderer>();

    private void Update()
    {
        if (_time < explodeDelay)
        {
            _time += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }

        UpdateColor(explosionColor, _time / explodeDelay * Time.deltaTime);
    }

    private void UpdateColor(Color color, float lerp) =>
        _meshRenderer.material.color = Color.Lerp(_meshRenderer.materials[0].color, color, lerp);
}