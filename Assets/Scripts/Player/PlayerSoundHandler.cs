using AdvancedSound;
using UnityEngine;

namespace Player
{
    public class PlayerSoundHandler : MonoBehaviour
    {
        [SerializeField] SoundPlayer Walk;
        [SerializeField] SoundPlayer Crouch;
        [SerializeField] SoundPlayer Run;
        [SerializeField] SoundPlayer Jump;
        [SerializeField] float RandomPitchRange;
        [SerializeField, Range(0, 1)] float WalkSpeedFootstepFloor = .2f;
        [SerializeField] float FootstepFrequencyMultiplier;

        Walk _walk;
        PlayerController _controller;
        float _footstepCounter;

        private void Start()
        {
            _controller = transform.parent.GetComponent<PlayerController>();
        }

        private void OnEnable()
        {
            _walk = transform.parent.GetComponent<Walk>();
            _walk.OnTrueJump += OnJump;
        }
        private void OnDisable()
        {
            _walk.OnTrueJump -= OnJump;
        }

        private void Update()
        {
            if (!_controller.UncoyotedGrounded) return; // no footsteps if youre not on the ground

            var vel = _controller.Body.linearVelocity;
            vel.y = 0;
            var speed = vel.magnitude;
            if (speed < WalkSpeedFootstepFloor ) return; // if youre really really slow no footsteps
            
            _footstepCounter += Mathf.Sqrt(speed) * FootstepFrequencyMultiplier * Time.fixedDeltaTime; // sqrt to make sprinting less frequent and crouching more frequent
            if (_footstepCounter < 1) return; // footsteps every so often

            _footstepCounter = 0;
            if(_walk.IsCrouched)
            {
                Crouch?.Play(1, GetRandomPitch());
                return;
            }
            if(_controller.SprintHeld)
            {
                Run?.Play(1, GetRandomPitch());
                return;
            }
            Walk?.Play(1, GetRandomPitch());
        }

        float GetRandomPitch() => 1 + (Random.value - .5f) * RandomPitchRange;

        void OnJump()
        {
            Jump?.Play(1, GetRandomPitch());
        }
    }
}
