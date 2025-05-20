using UnityEngine;

namespace Daytime
{
    public class DebugShowCamera : MonoBehaviour
    {
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            var cam = Camera.main;
            Gizmos.DrawFrustum(default, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, cam.aspect);
        }
    }
}
