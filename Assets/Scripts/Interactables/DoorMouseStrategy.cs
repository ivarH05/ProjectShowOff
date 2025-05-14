using Player;
using UnityEngine;

namespace Interactables
{
    public class DoorMouseStrategy : MouseStrategy
    {
        public float DoorSensitivity = 0.1f;
        public override void StartStrategy(PlayerController controller) { }
        public override void OnLook(PlayerController controller, Vector2 lookDelta)
        {
            if (!(controller.ActiveInteractable is Door door))
                return;

            if (door.IsLocked)
                return;


            door.Move(lookDelta.y * (door.isInFront ? -DoorSensitivity : DoorSensitivity));

        }
        public override void StopStrategy(PlayerController controller) { }
    }
}