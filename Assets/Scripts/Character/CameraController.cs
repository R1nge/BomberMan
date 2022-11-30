using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField] private Vector3 offset;
        private Camera _camera;

        private void Awake() => _camera = Camera.main;

        private void LateUpdate()
        {
            if (!IsOwner) return;
            _camera.transform.position = gameObject.transform.position + offset;
        }
    }
}