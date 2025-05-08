using Player;
using UnityEngine;

namespace Interactables
{
    public class DoorMouseStrategy : MouseStrategy
    {
        public override void StartStrategy(PlayerController controller) { }
        public override void OnLook(PlayerController controller, Vector2 lookDelta)
        {
            if (!(controller.ActiveInteractable is Door door))
                return;

            if (door.IsLocked)
                return;

            float DoorMoveSpeed = 0.2f;

            if (door.isInFront)
                DoorMoveSpeed = -DoorMoveSpeed;

            door.Move(lookDelta.y * DoorMoveSpeed);
        }
        public override void StopStrategy(PlayerController controller) { }
    }
}