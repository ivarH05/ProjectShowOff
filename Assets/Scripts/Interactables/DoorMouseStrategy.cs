using Player;
using UnityEngine;

namespace Interactables
{
    public class DoorMouseStrategy : MouseStrategy
    {
        [SerializeField] private float _doorSensitivity = 0.1f;
        [SerializeField] private float _peekMultiplier;
        private int peekDirection;
        [SerializeField] private CameraPeekController _peekController;
        public override void StartStrategy(PlayerController controller) { }
        public override void OnLook(PlayerController controller, Vector2 lookDelta)
        {
            if (!(controller.ActiveInteractable is Door door))
                return;

            if (door.IsLocked)
                return;


            door.Move(lookDelta.y * (door.isInFront ? -_doorSensitivity : _doorSensitivity) * door.SwingDirection);

        }
        public override void StopStrategy(PlayerController controller) { }


        public override void OnPeekLeftStart(PlayerController controller)
        {
            peekDirection -= 1;
            _peekController.SetDirection(peekDirection * _peekMultiplier);
        }

        public override void OnPeekLeftStop(PlayerController controller)
        {
            peekDirection += 1;
            _peekController.SetDirection(peekDirection * _peekMultiplier);
        }

        public override void OnPeekRightStart(PlayerController controller)
        {
            peekDirection += 1;
            _peekController.SetDirection(peekDirection * _peekMultiplier);
        }

        public override void OnPeekRightStop(PlayerController controller)
        {
            peekDirection -= 1;
            _peekController.SetDirection(peekDirection * _peekMultiplier);
        }
    }
}