// using UnityEngine;
// using ETouch = UnityEngine.InputSystem.EnhancedTouch;
//
// public class PlayerTouchMovement : MonoBehaviour
// {
//     [SerializeField] private Vector2 joystickSize = new(300, 300);
//     [SerializeField] private FloatingJoystick joystick;
//     private ETouch.Finger _movementFinger;
//     private Vector2 _movementAmount;
//
//     private void OnEnable()
//     {
//         ETouch.EnhancedTouchSupport.Enable();
//         ETouch.Touch.onFingerDown += HandleFingerDown;
//         ETouch.Touch.onFingerUp += HandleLoseFinger;
//         ETouch.Touch.onFingerMove += HandleFingerMove;
//     }
//
//     private void OnDisable()
//     {
//         ETouch.Touch.onFingerDown -= HandleFingerDown;
//         ETouch.Touch.onFingerUp -= HandleLoseFinger;
//         ETouch.Touch.onFingerMove -= HandleFingerMove;
//         ETouch.EnhancedTouchSupport.Disable();
//     }
//
//     private void HandleFingerMove(ETouch.Finger movedFinger)
//     {
//         if (movedFinger == _movementFinger)
//         {
//             Vector2 knobPosition;
//             var maxMovement = joystickSize.x / 2f;
//             var currentTouch = movedFinger.currentTouch;
//
//             if (Vector2.Distance(
//                     currentTouch.screenPosition,
//                     joystick.RectTransform.anchoredPosition
//                 ) > maxMovement)
//             {
//                 knobPosition = (
//                                    currentTouch.screenPosition - joystick.RectTransform.anchoredPosition
//                                ).normalized
//                                * maxMovement;
//             }
//             else
//             {
//                 knobPosition = currentTouch.screenPosition - joystick.RectTransform.anchoredPosition;
//             }
//
//             joystick.Knob.anchoredPosition = knobPosition;
//             _movementAmount = knobPosition / maxMovement;
//         }
//     }
//
//     private void HandleLoseFinger(ETouch.Finger lostFinger)
//     {
//         if (lostFinger == _movementFinger)
//         {
//             _movementFinger = null;
//             joystick.Knob.anchoredPosition = Vector2.zero;
//             joystick.gameObject.SetActive(false);
//             _movementAmount = Vector2.zero;
//         }
//     }
//
//     private void HandleFingerDown(ETouch.Finger touchedFinger)
//     {
//         if (_movementFinger == null && touchedFinger.screenPosition.x <= Screen.width / 2f)
//         {
//             _movementFinger = touchedFinger;
//             _movementAmount = Vector2.zero;
//             joystick.gameObject.SetActive(true);
//             joystick.RectTransform.sizeDelta = joystickSize;
//             joystick.RectTransform.anchoredPosition = ClampStartPosition(touchedFinger.screenPosition);
//         }
//     }
//
//     private Vector2 ClampStartPosition(Vector2 startPosition)
//     {
//         if (startPosition.x < joystickSize.x / 2)
//         {
//             startPosition.x = joystickSize.x / 2;
//         }
//
//         if (startPosition.y < joystickSize.y / 2)
//         {
//             startPosition.y = joystickSize.y / 2;
//         }
//         else if (startPosition.y > Screen.height - joystickSize.y / 2)
//         {
//             startPosition.y = Screen.height - joystickSize.y / 2;
//         }
//
//         return startPosition;
//     }
//
//     private void Update()
//     {
//         // Vector3 scaledMovement = Player.speed * Time.deltaTime * new Vector3(
//         //     _movementAmount.x,
//         //     0,
//         //     _movementAmount.y
//         // );
//     }
// }