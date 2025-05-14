using UnityEngine;

namespace Interactables
{
    public class Coffin : Door
    {

        public override void GetCameraTransforms(out Vector3 PositionResult, out Vector3 DirectionResult)
        {
            if (isInFront)
            {
                PositionResult = new Vector3(1.1f, 0.6f, Mathf.Lerp(-0.2f, 0.3f, hinge.localEulerAngles.y / 90));

                DirectionResult = Vector3.Lerp(new Vector3(15, -50, -15), new Vector3(-10, -90, 0), hinge.localEulerAngles.y / 180);
            }
            else
            {
                PositionResult = Vector3.Lerp(new Vector3(0.3f, 0.6f, 0.3f), new Vector3(0.2f, 0.6f, 0.5f), hinge.localEulerAngles.y / 180);
                DirectionResult = Vector3.Lerp(new Vector3(0, 120, -20), new Vector3(0, 0, 0), hinge.localEulerAngles.y / 180);
            }
        }
    }
}
