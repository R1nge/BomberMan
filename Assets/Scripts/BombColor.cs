using UnityEngine;

public class BombColor : MonoBehaviour
{
    [SerializeField] private float explodeDelay;
    [SerializeField] private Color explosionColor;
    private float _time;
    private MeshRenderer _meshRenderer;

    private void Awake() => _meshRenderer = GetComponent<MeshRenderer>();

    //https://docs-multiplayer.unity3d.com/netcode/current/advanced-topics/networktime-ticks/#example-1-using-network-time-to-synchronize-environments
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