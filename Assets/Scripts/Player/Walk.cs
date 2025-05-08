using UnityEngine;

namespace Player
{
    /// <summary>
    /// Allows walking around. 
    /// </summary>
    public class Walk : MovementStrategy
    {
        [SerializeField] float _speed = 2;
        [SerializeField] float _sprintMultiplier = 2;
        [SerializeField] float _jumpForce = 3;
        [SerializeField] float _jumpCooldown = .5f;
        [SerializeField, Range(0.01f,1)] float _inAirSpeedMultiplier = .4f;
        [SerializeField] float _turnSmoothness = 10f;
        float _currentJumpCooldown;

        public override void StartStrategy(PlayerController controller) {}
        public override void StopStrategy(PlayerController controller) {}

        public override void OnMoveUpdate(PlayerController controller, Vector3 direction, bool sprintHeld, bool crouchHeld)
        {
            _currentJumpCooldown += Time.deltaTime;
            var vel = direction * _speed * (sprintHeld ? _sprintMultiplier : 1);
            var curvel = controller.Body.linearVelocity;
            vel.y = curvel.y;
            float lerpval = 1 - Mathf.Exp(Time.deltaTime * -_turnSmoothness / (controller.UncoyotedGrounded ? 1 : _inAirSpeedMultiplier));
            controller.Body.linearVelocity = Vector3.LerpUnclamped(vel, curvel, lerpval);
        }
        public override void OnJump(PlayerController controller)
        {
            if(controller.IsGrounded && _currentJumpCooldown > _jumpCooldown)
            {
                _currentJumpCooldown = 0;
                controller.Body.AddForce(new Vector3(0,_jumpForce,0), ForceMode.Impulse);
            }
        }
    }
}
