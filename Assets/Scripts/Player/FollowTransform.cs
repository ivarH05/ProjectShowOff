using UnityEngine;

namespace Player
{
    public class FollowTransform : MonoBehaviour
    {
        [SerializeField] float _followStrength = 50;
        [SerializeField] Transform _toFollow;
        void LateUpdate()
        {
            if (_toFollow == null) return;

            var lerpval = 1-Mathf.Exp(Time.deltaTime * -_followStrength);
            var currRot = transform.rotation;
            var currPos = transform.position;
            var tgtRot = _toFollow.rotation;
            var tgtPos = _toFollow.position;
            transform.rotation = Quaternion.Slerp(currRot, tgtRot, lerpval);
            transform.position = Vector3.LerpUnclamped(currPos, tgtPos, lerpval);
        }
    }
}
