
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Simple script for looking around with the camera. Slightly jank but does the job.
    /// </summary>
    public class LookAround : MouseStrategy
    {
        [SerializeField] float _maximumVerticalAngle = 90;
        [SerializeField] float _sensitivity = .05f;
        [SerializeField] bool _flipVertical = true;

        [SerializeField] private CameraPeekController _peekController;

        private int peekDirection;

        Vector2 _currentMouse = new Vector2();

        public override void StartStrategy(PlayerController controller) 
        {
            _currentMouse = new Vector2(
                controller.transform.localEulerAngles.y + controller.CameraTransform.localEulerAngles.y, 
                controller.CameraTransform.eulerAngles.x);
        }

        public override void StopStrategy(PlayerController controller)
        {
            peekDirection = 0;
            _peekController.SetDirection(0);
        }

        public override void OnLook(PlayerController controller, Vector2 lookDelta)
        {
            if(_flipVertical) lookDelta.y = -lookDelta.y;

            _currentMouse += lookDelta * _sensitivity;
            _currentMouse.y = ClampedAngle(_currentMouse.y);
            
            controller.Body.rotation = Quaternion.Euler(0, _currentMouse.x, 0);
            controller.CameraTransform.localRotation = Quaternion.Euler(_currentMouse.y, 0, 0);
        }

        public override void OnPeekLeftStart(PlayerController controller)
        {
            peekDirection -= 1;
            _peekController.SetDirection(peekDirection);
        }

        public override void OnPeekLeftStop(PlayerController controller)
        {
            peekDirection += 1;
            _peekController.SetDirection(peekDirection);
        }

        public override void OnPeekRightStart(PlayerController controller)
        {
            peekDirection += 1;
            _peekController.SetDirection(peekDirection);
        }

        public override void OnPeekRightStop(PlayerController controller)
        {
            peekDirection -= 1;
            _peekController.SetDirection(peekDirection);
        }

        private float ClampedAngle(float angle) => Mathf.Clamp(angle, -_maximumVerticalAngle, _maximumVerticalAngle);
    }
}