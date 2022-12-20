using Unity.Netcode;
using UnityEngine;

public class BombColor : NetworkBehaviour
{
    [SerializeField] private float explodeDelay;
    [SerializeField] private Color explosionColor;
    private MeshRenderer _meshRenderer;
    private double _time;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        NetworkManager.NetworkTickSystem.Tick += OnTick;
    }

    private void OnTick()
    {
        if (_time < explodeDelay)
        {
            _time += NetworkManager.Singleton.LocalTime.FixedDeltaTime;
            UpdateColorClientRpc(explosionColor,
                _time / explodeDelay * NetworkManager.Singleton.LocalTime.FixedDeltaTime);
        }
    }

    [ClientRpc]
    private void UpdateColorClientRpc(Color color, double lerp) =>
        _meshRenderer.material.color = Color.Lerp(_meshRenderer.materials[0].color, color, (float)lerp);

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.NetworkTickSystem.Tick -= OnTick;
    }
}