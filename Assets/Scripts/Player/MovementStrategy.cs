using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public abstract class MovementStrategy : MonoBehaviour
    {
        public abstract void StartStrategy(PlayerController controller);
        public abstract void OnMoveUpdate(PlayerController controller, Vector3 direction, bool sprintHeld, CrouchState crouchHeld);
        public abstract void OnJump(PlayerController controller);
        public abstract void StopStrategy(PlayerController controller);
    }
}
