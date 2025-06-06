using UnityEngine;
using UnityEngine.Events;

namespace Daytime
{
    public class TimeHandler : MonoBehaviour
    {
        [SerializeField] int _maximumCharactersTalkedTo;
        [SerializeField] float _animatorSpeed = 5;
        [SerializeField] Animator _dayProgressAnimation;
        public UnityEvent OnNight;

        int _charactersTalkedTo = 0;
        float _progress = 0;

        private void Start()
        {
            _dayProgressAnimation.StopPlayback();
        }

        /// <summary>
        /// Registers a character being talked to and advances the clock.
        /// </summary>
        public void TalkToCharacter()
        {
            _charactersTalkedTo++;
            if(_charactersTalkedTo >= _maximumCharactersTalkedTo)
                OnNight?.Invoke();
        }

        private void Update()
        {
            float tgtProgress = (float)_charactersTalkedTo / _maximumCharactersTalkedTo;
            float diff = tgtProgress - _progress;
            diff *= 1 - Mathf.Exp(-Time.deltaTime * _animatorSpeed);
            _progress += diff;
            _dayProgressAnimation.Update(diff);
        }
    }
}
