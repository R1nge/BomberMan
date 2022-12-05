using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class MovementController : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<float> speed;
        [SerializeField] private float rotationSpeed;
        private NetworkVariable<bool> _canMove;
        private Vector3 _moveDirection = Vector3.zero;
        private CharacterController _characterController;
        private GameState _gameState;

        private void Awake()
        {
            _canMove = new NetworkVariable<bool>();
            _characterController = GetComponent<CharacterController>();
            _gameState = FindObjectOfType<GameState>();
        }

        public override void OnNetworkSpawn()
        {
            if(!IsOwner) return;
            _gameState.OnGameStarted += OnGameStartedServerRpc;
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnGameStartedServerRpc() => _canMove.Value = true;

        private void Update()
        {
            if (!IsOwner || !_canMove.Value) return;
            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;
            float curSpeedX = _canMove.Value ? speed.Value * Input.GetAxis("Vertical") : 0;
            float curSpeedY = _canMove.Value ? speed.Value * Input.GetAxis("Horizontal") : 0;
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

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_gameState == null) return;
            _gameState.OnGameStarted -= OnGameStartedServerRpc;
        }
    }
}