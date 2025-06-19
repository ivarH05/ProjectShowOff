using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public abstract class MouseStrategy : MonoBehaviour
    {
        public abstract void StartStrategy(PlayerController controller);
        public abstract void OnLook(PlayerController controller, Vector2 lookDelta);

        public abstract void StopStrategy(PlayerController controller);

        public abstract void OnPeekLeftStart(PlayerController controller);
        public abstract void OnPeekLeftStop(PlayerController controller);
        public abstract void OnPeekRightStart(PlayerController controller);
        public abstract void OnPeekRightStop(PlayerController controller);
    }
}
