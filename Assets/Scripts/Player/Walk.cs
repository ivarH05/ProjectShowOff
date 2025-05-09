using System.Collections;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Allows walking around. 
    /// </summary>
    public class Walk : MovementStrategy
    {
        [SerializeField] float _speed = 2;
        [SerializeField, Range(0.01f,1)] float _inAirSpeedMultiplier = .4f;
        [SerializeField] float _turnSmoothness = 10f;

        [SerializeField] float _sprintMultiplier = 2;
        
        [SerializeField] float _crouchSpeedMultiplier = .3f;
        [SerializeField] float _crouchHeightMultiplier = .4f;
        [SerializeField] float _crouchJumpHeightMultiplier = .2f;
        [SerializeField] float _crouchAnimationSpeed = 1f;
        
        [SerializeField] float _jumpForce = 3;
        [SerializeField] float _jumpCooldown = .5f;
        
        float _currentJumpCooldown;
        float _crouchValue01 = 0; // 0 if not crouching, 1 if crouching, anything in between for transitionary states

        public override void StartStrategy(PlayerController controller) {}
        public override void StopStrategy(PlayerController controller) 
        {
            controller.StartCoroutine(StopCrouch());
            IEnumerator StopCrouch()
            {
                float myHeight = controller.MainCollider.height;
                while(controller.MainCollider.height != controller.CharacterHeight)
                {
                    if (myHeight != controller.MainCollider.height)
                        break;
                    UpdateCrouch(false, controller);
                    myHeight = controller.MainCollider.height; 
                    yield return new WaitForFixedUpdate();
                }
                Debug.Log("reached end");
            }
        }

        public override void OnMoveUpdate(PlayerController controller, Vector3 direction, bool sprintHeld, bool crouchHeld)
        {
            if(_crouchValue01 > .2f) sprintHeld = false;

            _currentJumpCooldown += Time.deltaTime;
            var vel = direction * _speed * (sprintHeld ? _sprintMultiplier : Mathf.Lerp(1, _crouchSpeedMultiplier, _crouchValue01));
            var curvel = controller.Body.linearVelocity;
            vel.y = curvel.y;
            float lerpval = 1 - Mathf.Exp(Time.deltaTime * -_turnSmoothness / (controller.UncoyotedGrounded ? 1 : _inAirSpeedMultiplier));
            controller.Body.linearVelocity = Vector3.LerpUnclamped(vel, curvel, lerpval);

            UpdateCrouch(crouchHeld, controller);
        }
        public override void OnJump(PlayerController controller)
        {
            if(controller.IsGrounded && _currentJumpCooldown > _jumpCooldown)
            {
                _currentJumpCooldown = 0;
                controller.Body.AddForce(new Vector3(0,_jumpForce * Mathf.Lerp(1, _crouchJumpHeightMultiplier, _crouchValue01), 0), ForceMode.Impulse);
            }
        }
        void UpdateCrouch(bool crouchHeld, PlayerController controller)
        {
            if (crouchHeld)
                _crouchValue01 += (1-_crouchValue01) * (1 - Mathf.Exp(-Time.deltaTime * _crouchAnimationSpeed));
            else _crouchValue01 -= Time.deltaTime * _crouchAnimationSpeed;
            _crouchValue01 = Mathf.Clamp01(_crouchValue01);

            float crouchedHeight = controller.CharacterHeight * _crouchHeightMultiplier;
            float currentHeight = Mathf.Lerp(controller.CharacterHeight, crouchedHeight, _crouchValue01);
            float heightDelta = currentHeight - controller.MainCollider.height;

            Vector3 headTipPos = controller.transform.position + Vector3.up * (controller.MainCollider.height*.5f+controller.MainCollider.center.y);
            var ray = new Ray(headTipPos - Vector3.up * .2f, Vector3.up);
            if (heightDelta > 0 && Physics.Raycast(ray, .4f))
            {
                Debug.DrawRay(ray.origin, ray.direction * .4f);
                _crouchValue01 = Mathf.Clamp01(_crouchValue01 + Time.deltaTime * _crouchAnimationSpeed);
                return;
            }
            else Debug.DrawRay(ray.origin, ray.direction * .4f, Color.red);

            controller.MainCollider.height = currentHeight;
            controller.MainCollider.center += new Vector3(0, heightDelta*.5f, 0);
            controller.CameraTransform.position += new Vector3(0, heightDelta, 0);
        }
    }
}
