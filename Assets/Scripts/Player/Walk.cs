using UnityEngine;

namespace Player
{
    /// <summary>
    /// Allows walking around. No jumping.
    /// </summary>
    public class Walk : MovementStrategy
    {
        [SerializeField] float _speed = 2;

        public override void StartStrategy(PlayerController controller) {}

        public override void OnMove(PlayerController controller, Vector3 direction)
        {
            controller.Body.MovePosition(controller.Body.position + direction * Time.deltaTime * _speed);
        }

        public override void StopStrategy(PlayerController controller) {}
    }
}
