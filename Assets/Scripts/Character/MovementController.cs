using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class MovementController : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<float> speed;
        [SerializeField] private float rotationSpeed;
        private bool _canMove;
        private Vector3 _moveDirection = Vector3.zero;
        private CharacterController _characterController;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            StartCoroutine(Wait_C());
        }

        private IEnumerator Wait_C()
        {
            yield return new WaitForSeconds(1);
            _canMove = true;
        }

        private void Update()
        {
            if (!IsOwner || !_canMove) return;
            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;
            float curSpeedX = _canMove ? speed.Value * Input.GetAxis("Vertical") : 0;
            float curSpeedY = _canMove ? speed.Value * Input.GetAxis("Horizontal") : 0;
            _moveDirection = forward * curSpeedX + right * curSpeedY;

            if (_moveDirection != Vector3.zero)
            {
                var targetRot = Quaternion.LookRotation(_moveDirection, Vector3.up);
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            _characterController.Move(_moveDirection * Time.deltaTime);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetSpeedServerRpc(float value) => speed.Value = value;
    }
}