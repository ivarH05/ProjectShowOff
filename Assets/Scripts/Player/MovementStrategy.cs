using UnityEngine;

namespace Player
{
    public abstract class MovementStrategy : MonoBehaviour
    {
        public abstract void StartStrategy(PlayerController controller);
        public abstract void OnMove(PlayerController controller, Vector3 direction);
        public abstract void StopStrategy(PlayerController controller);
    }
}
