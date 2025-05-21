using UnityEngine;

namespace Player
{
    public class FollowTransform : MonoBehaviour
    {
        [SerializeField] float _followStrength = 50;
        [field: SerializeField] public Transform ToFollow { get; private set; }
        void LateUpdate()
        {
            if (ToFollow == null) return;

            var lerpval = 1-Mathf.Exp(Time.deltaTime * -_followStrength);
            var currRot = transform.rotation;
            var currPos = transform.position;
            var tgtRot = ToFollow.rotation;
            var tgtPos = ToFollow.position;
            transform.rotation = Quaternion.Slerp(currRot, tgtRot, lerpval);
            transform.position = Vector3.LerpUnclamped(currPos, tgtPos, lerpval);
        }
    }
}
