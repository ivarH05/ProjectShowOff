using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public abstract class MouseStrategy : MonoBehaviour
    {
        public abstract void StartStrategy(PlayerController controller);
        public abstract void OnLook(PlayerController controller, Vector2 lookDelta);
        public abstract void OnAttack(PlayerController controller);
        public abstract void OnAttackSecondary(PlayerController controller);
        public abstract void OnInteract(PlayerController controller);
        public abstract void StopStrategy(PlayerController controller);
    }
}
