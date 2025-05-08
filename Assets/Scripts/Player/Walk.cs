using UnityEngine;

namespace Player
{
    /// <summary>
    /// Allows walking around. No jumping.
    /// </summary>
    public class Walk : MovementStrategy
    {
        [SerializeField] float _speed = 2;
        [SerializeField] float _sprintMultiplier = 2;
        [SerializeField] float _jumpForce = 3;

        public override void StartStrategy(PlayerController controller) {}

        public override void OnMove(PlayerController controller, Vector3 direction, bool sprintHeld)
        {
            controller.Body.MovePosition(controller.Body.position + direction * Time.deltaTime * _speed * (sprintHeld ? 1 : _sprintMultiplier));
        }
        public override void OnJump(PlayerController controller)
        {
            controller.Body.AddForce(new Vector3(0,_jumpForce,0), ForceMode.Impulse);
        }

        public override void StopStrategy(PlayerController controller) {}

    }
}
