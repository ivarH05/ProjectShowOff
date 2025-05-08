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
        [SerializeField] float _jumpCooldown = .5f;
        float _currentJumpCooldown;

        public override void StartStrategy(PlayerController controller) {}

        public override void OnMoveUpdate(PlayerController controller, Vector3 direction, bool sprintHeld, bool crouchHeld)
        {
            _currentJumpCooldown += Time.deltaTime;
            controller.Body.MovePosition(controller.Body.position + direction * Time.deltaTime * _speed * (sprintHeld ? _sprintMultiplier : 1));
        }
        public override void OnJump(PlayerController controller)
        {
            if(controller.IsGrounded && _currentJumpCooldown > _jumpCooldown)
            {
                _currentJumpCooldown = 0;
                controller.Body.AddForce(new Vector3(0,_jumpForce,0), ForceMode.Impulse);
            }
        }

        public override void StopStrategy(PlayerController controller) {}

    }
}
