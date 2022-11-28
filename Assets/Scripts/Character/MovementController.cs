using UnityEngine;

namespace Character
{
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private bool canMove = true;
        private Vector3 _moveDirection = Vector3.zero;
        private CharacterController _characterController;

        private void Awake() => _characterController = GetComponent<CharacterController>();

        private void Update()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
            _moveDirection = forward * curSpeedX + right * curSpeedY;

            _characterController.Move(_moveDirection * Time.deltaTime);
        }
    }
}