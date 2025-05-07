using UnityEngine;

namespace Player
{
    public class LookAroundStrategy : MouseStrategy
    {
        [SerializeField] float _maximumVerticalAngle = 90;
        [SerializeField] float _sensitivity = .05f;
        [SerializeField] bool _flipVertical = true;

        Vector2 _currentMouse;

        public override void StartStrategy(PlayerController controller) { _currentMouse = controller.CameraTransform.eulerAngles; }
        public override void StopStrategy(PlayerController controller) { }

        public override void OnLook(PlayerController controller, Vector2 lookDelta)
        {
            if(_flipVertical) lookDelta.y = -lookDelta.y;

            _currentMouse += lookDelta * _sensitivity;
            _currentMouse.y = Mathf.Clamp(_currentMouse.y, -_maximumVerticalAngle, _maximumVerticalAngle);
            
            controller.transform.localRotation = Quaternion.Euler(0, _currentMouse.x, 0);
            controller.CameraTransform.localRotation = Quaternion.Euler(_currentMouse.y, 0, 0);
        }
    }
}