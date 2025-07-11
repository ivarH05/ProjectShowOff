using AdvancedSound;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerSoundHandler : MonoBehaviour
    {
        [SerializeField] SoundPlayer Walk;
        [SerializeField] SoundPlayer Crouch;
        [SerializeField] SoundPlayer Run;
        [SerializeField] SoundPlayer Jump;
        [SerializeField] SoundPlayer CrouchFast;

        [SerializeField] float RandomPitchRange;
        [SerializeField, Range(0, 1)] float WalkSpeedFootstepFloor = .2f;
        [SerializeField] float FootstepFrequencyMultiplier;

        [SerializeField] LoudnessIndicator _loudnessIndicator;
        [SerializeField] float _indicatorFadeoutSpeed = 1;
        [SerializeField] float _indicatorSensitivity = 1;
        [SerializeField] float _indicatorSmoothness = 2;

        Walk _walk;
        PlayerController _controller;
        float _footstepCounter;
        float _currentLoudness;
        float _smoothCurrentLoudness;

        /// <summary>
        /// Adds a sound to the handler, and displays how loud it was to the player.
        /// </summary>
        /// <param name="range">The sound's faint range.</param>
        public void AddSound(float range)
        {
            float loudness = 1 - Mathf.Exp(-range * _indicatorSensitivity);
            _currentLoudness = Mathf.Max(_currentLoudness, loudness);
        }

        private void OnEnable()
        {
            _walk = transform.parent.GetComponent<Walk>();
            _controller = transform.parent.GetComponent<PlayerController>();
            _walk.OnTrueJump += OnJump;
            _controller.OnFastCrouch += OnCrouchFast;
            if (_loudnessIndicator == null) 
                Debug.LogError("PlayerSoundHandler does not have a loudnessindicator set. Assign it in the inspector please", this);
        }
        private void OnDisable()
        {
            _walk.OnTrueJump -= OnJump;
        }

        private void Update()
        {
            UpdateLoudnessIndicator();

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
                if (Crouch == null) return;
                Crouch.Play(1, GetRandomPitch());
                AddSound(Crouch.FaintRangeAtFullVolume);
                return;
            }
            if(_controller.SprintHeld)
            {
                if (Run == null) return;
                Run.Play(1, GetRandomPitch());
                AddSound(Run.FaintRangeAtFullVolume);
                return;
            }
            if(Walk == null) return;
            Walk.Play(1, GetRandomPitch());
            AddSound(Walk.FaintRangeAtFullVolume);
        }

        float GetRandomPitch() => 1 + (Random.value - .5f) * RandomPitchRange;

        void UpdateLoudnessIndicator()
        {
            if(_loudnessIndicator == null) return;
            _currentLoudness -= Time.deltaTime * _indicatorFadeoutSpeed;
            _currentLoudness = Mathf.Clamp01(_currentLoudness);
            _smoothCurrentLoudness -= (_smoothCurrentLoudness - _currentLoudness) * (1- Mathf.Exp(-Time.deltaTime * _indicatorSmoothness));
            _loudnessIndicator.Loudness = _smoothCurrentLoudness;
        }

        void OnJump()
        {
            if(Jump == null) return;
            Jump.Play(1, GetRandomPitch());
            AddSound(Jump.FaintRangeAtFullVolume);
        }

        void OnCrouchFast()
        {
            if (CrouchFast == null) return;
            CrouchFast.Play(1, GetRandomPitch());
            AddSound(CrouchFast.FaintRangeAtFullVolume);
        }
    }
}
