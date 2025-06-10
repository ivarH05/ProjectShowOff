using UnityEngine;
using UnityEngine.InputSystem;

namespace Daytime
{
    [RequireComponent(typeof(Camera))]
    public class CameraMouseSelector : MonoBehaviour
    {
        FromCameraSelectable _lastHovered;
        Camera _cam;

        private void OnEnable()
        {
            _cam = GetComponent<Camera>();
        }
        private void OnDisable()
        {
            if(_lastHovered != null)
            {
                _lastHovered.OnHoverEnd?.Invoke();
                _lastHovered = null;
            }
        }

        private void Update()
        {
            var mouse = Mouse.current;
            if(mouse.leftButton.wasPressedThisFrame)
                _lastHovered?.OnClicked?.Invoke();
        }

        private void FixedUpdate()
        {
            var mouse = Mouse.current;
            var ray = _cam.ScreenPointToRay(mouse.position.ReadValue());
            if(
                Physics.Raycast(ray, out var info, float.MaxValue, 1 << 7) &&
                info.transform.TryGetComponent<FromCameraSelectable>(out var comp) &&
                comp.enabled)
            {
                if(_lastHovered == comp)
                {
                    comp.WhileHoverFixedUpdate?.Invoke();
                    return;
                }
                else
                {
                    _lastHovered?.OnHoverEnd?.Invoke();
                    _lastHovered = comp;
                }
                comp.OnHoverStart?.Invoke();
            }
            else if (_lastHovered != null)
            {
                _lastHovered.OnHoverEnd?.Invoke();
                _lastHovered = null;
            }
        }
    }
}
