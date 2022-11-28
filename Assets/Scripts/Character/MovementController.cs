using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class MovementController : NetworkBehaviour
    {
        [SerializeField] private float speed;
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
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = _canMove ? speed * Input.GetAxis("Vertical") : 0;
            float curSpeedY = _canMove ? speed * Input.GetAxis("Horizontal") : 0;
            _moveDirection = forward * curSpeedX + right * curSpeedY;

            _characterController.Move(_moveDirection * Time.deltaTime);
        }
    }
}