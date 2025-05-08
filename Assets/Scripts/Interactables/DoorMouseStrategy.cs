using Player;
using UnityEngine;

namespace Interactables
{
    public class DoorMouseStrategy : MouseStrategy
    {
        private Door door;
        public DoorMouseStrategy(Door door)
        {
            this.door = door;
        }
        
        public override void StartStrategy(PlayerController controller) { }
        public override void OnLook(PlayerController controller, Vector2 lookDelta)
        {
            if (door.IsLocked)
                return;

            float DoorMoveSpeed = 0.2f;

            if (door.IsInFront(controller.CameraTransform.position))
                DoorMoveSpeed = -DoorMoveSpeed;

            door.Move(lookDelta.y * DoorMoveSpeed);
        }
        public override void OnAttack(PlayerController controller) { }
        public override void OnAttackSecondary(PlayerController controller) { }
        public override void StopStrategy(PlayerController controller) { }
    }
}