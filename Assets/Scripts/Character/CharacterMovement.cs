using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class CharacterMovement : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<float> speed;
        [SerializeField] private float rotationSpeed;
        private NetworkVariable<bool> _canMove;
        private Vector3 _moveDirection = Vector3.zero;
        private CharacterController _characterController;
        private GameState _gameState;
        private float _curSpeedX, _curSpeedY;

        private void Awake()
        {
            _canMove = new NetworkVariable<bool>();
            _characterController = GetComponent<CharacterController>();
            _gameState = FindObjectOfType<GameState>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            _gameState.OnGameStarted += OnGameStartedServerRpc;
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnGameStartedServerRpc() => _canMove.Value = true;

        public void OnMove(InputValue value)
        {
            if (!_canMove.Value) return;
            _curSpeedX = value.Get<Vector2>().y * speed.Value;
            _curSpeedY = value.Get<Vector2>().x * speed.Value;
        }

        private void Update()
        {
            if (!IsOwner || !_canMove.Value) return;

            _moveDirection = Vector3.forward * _curSpeedX + Vector3.right * _curSpeedY;

            Rotate();

            _characterController.Move(_moveDirection * Time.deltaTime);
        }

        private void Rotate()
        {
            if (_moveDirection != Vector3.zero)
            {
                var targetRot = Quaternion.LookRotation(_moveDirection, Vector3.up);
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
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