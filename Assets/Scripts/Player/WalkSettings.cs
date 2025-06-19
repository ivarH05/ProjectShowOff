using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "WalkSettings", menuName = "Scriptable Objects/WalkSettings")]
    public class WalkSettings : ScriptableObject
    {
        public float Speed = 2;
        [Range(0.01f, 1)] public float InAirSpeedMultiplier = .4f;
        public float TurnSmoothness = 10f;

        public float SprintMultiplier = 2;

        public float CrouchSpeedMultiplier = .3f;
        public float CrouchHeightMultiplier = .4f;
        public float CrouchJumpHeightMultiplier = .2f;
        public float CrouchAnimationSpeed = 1f;
        public float FastCrouchSpeedMult = 3f;

        public float JumpForce = 3;
        public float JumpCooldown = .5f;
    }
}
