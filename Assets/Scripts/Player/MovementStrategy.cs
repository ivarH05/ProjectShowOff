using UnityEngine;

namespace Player
{
    public abstract class MovementStrategy : MonoBehaviour
    {
        public abstract void StartStrategy(PlayerController controller);
        public abstract void OnMoveUpdate(PlayerController controller, Vector3 direction, bool sprintHeld, bool crouchHeld);
        public abstract void OnJump(PlayerController controller);
        public abstract void StopStrategy(PlayerController controller);
    }
}
