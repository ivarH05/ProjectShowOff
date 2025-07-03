using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Allows walking around. 
    /// </summary>
    public class Walk : MovementStrategy
    {
        [SerializeField] WalkSettings _settings;

        public event Action OnTrueJump;
        public bool IsCrouched => _crouchValue01 > .5f;

        float _currentJumpCooldown;
        float _crouchValue01 = 0; // 0 if not crouching, 1 if crouching, anything in between for transitionary states

        public override void StartStrategy(PlayerController controller) 
        {
            if (_settings == null) Debug.LogError("Walk settings were null! The player will not be able to move around.", this);
        }
        public override void StopStrategy(PlayerController controller) 
        {
            if (_settings == null) return;

            controller.StartCoroutine(StopCrouch());
            IEnumerator StopCrouch()
            {
                float myHeight = controller.MainCollider.height;
                while(controller.MainCollider.height != controller.CharacterHeight)
                {
                    if (myHeight != controller.MainCollider.height)
                        break;
                    UpdateCrouch(CrouchState.Standing, controller);
                    myHeight = controller.MainCollider.height;
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        public override void OnMoveUpdate(PlayerController controller, Vector3 direction, bool sprintHeld, CrouchState crouchHeld)
        {
            if (_settings == null) return;

            if(_crouchValue01 > .2f) sprintHeld = false;

            _currentJumpCooldown += Time.deltaTime;
            var vel = direction * _settings.Speed * (sprintHeld ? _settings.SprintMultiplier : Mathf.Lerp(1, _settings.CrouchSpeedMultiplier, _crouchValue01));
            var curvel = controller.Body.linearVelocity;
            vel.y = curvel.y;
            float lerpval = 1 - Mathf.Exp(Time.deltaTime * -_settings.TurnSmoothness / (controller.UncoyotedGrounded ? 1 : _settings.InAirSpeedMultiplier));
            controller.Body.linearVelocity = Vector3.LerpUnclamped(vel, curvel, lerpval);

            UpdateCrouch(crouchHeld, controller);
        }
        public override void OnJump(PlayerController controller)
        {
            if (_settings == null) return;

            if(controller.IsGrounded && _currentJumpCooldown > _settings.JumpCooldown)
            {
                _currentJumpCooldown = 0;
                controller.Body.AddForce(new Vector3(0, _settings.JumpForce * Mathf.Lerp(1, _settings.CrouchJumpHeightMultiplier, _crouchValue01), 0), ForceMode.Impulse);
                OnTrueJump?.Invoke();
            }
        }
        void UpdateCrouch(CrouchState crouchHeld, PlayerController controller)
        {
            if (_settings == null) return;

            Debug.Log(crouchHeld);

            float speedMult = crouchHeld == CrouchState.CrouchFast ? _settings.FastCrouchSpeedMult : 1;

            if (crouchHeld != CrouchState.Standing)
                _crouchValue01 += (1-_crouchValue01) * (1 - Mathf.Exp(-Time.deltaTime * _settings.CrouchAnimationSpeed * speedMult));
            else _crouchValue01 -= Time.deltaTime * _settings.CrouchAnimationSpeed;
            _crouchValue01 = Mathf.Clamp01(_crouchValue01);

            float crouchedHeight = controller.CharacterHeight * _settings.CrouchHeightMultiplier;
            float currentHeight = Mathf.Lerp(controller.CharacterHeight, crouchedHeight, _crouchValue01);
            float heightDelta = currentHeight - controller.MainCollider.height;

            float headTipOffset = (controller.MainCollider.height*.5f + controller.MainCollider.center.y);
            Vector3 headTipPos = controller.transform.position + Vector3.up * headTipOffset;
            var ray = new Ray(headTipPos - Vector3.up * .2f, Vector3.up);
            if (heightDelta > 0 && Physics.Raycast(ray, .4f))
            {
                _crouchValue01 = Mathf.Clamp01(_crouchValue01 + Time.deltaTime * _settings.CrouchAnimationSpeed * speedMult);
                return;
            }

            controller.MainCollider.height = currentHeight;
            controller.MainCollider.center += new Vector3(0, heightDelta*.5f, 0);
            controller.CrouchTransform.localPosition = new Vector3(0,headTipOffset + controller.EyeOffset,0);
        }
    }
}
