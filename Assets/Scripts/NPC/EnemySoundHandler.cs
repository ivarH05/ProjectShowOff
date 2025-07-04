using System.Collections;
using AdvancedSound;
using UnityEngine;

namespace NPC
{
    public class EnemySoundHandler : MonoBehaviour
    {
        [SerializeField] SoundPlayer _onNotice;
        [SerializeField] SoundPlayer _footStep;
        [SerializeField] SoundPlayer _medalClinking;
        [SerializeField] float _moveStepThreshold = 1f;
        [SerializeField] float _stepFrequency = 1f;
        [SerializeField] float _medalClinkRandomOffset = .4f;
        [SerializeField] float _medalClinkConstantOffset = .4f;

        Enemy _enemy;
        float _distanceMoved;
        Vector3 _previousPosition;

        private void OnEnable()
        {
            _enemy = transform.parent.GetComponent<Enemy>();
            _enemy.events.OnChangeState.AddListener(OnChangeState);
            _previousPosition = _enemy.transform.position;
        }
        private void OnDisable()
        {
            _enemy.events.OnChangeState.RemoveListener(OnChangeState);
        }

        private void FixedUpdate()
        {
            Vector3 currentPosition = _enemy.transform.position;
            float posDelta = (currentPosition - _previousPosition).magnitude;
            _previousPosition = currentPosition;
            if (posDelta < _moveStepThreshold * Time.fixedDeltaTime)
                return;

            _distanceMoved += _stepFrequency * posDelta;
            if(_distanceMoved > 1)
            {
                _distanceMoved = 0;
                _footStep.Play();
                StartCoroutine(startClinkRandomOffset());

                IEnumerator startClinkRandomOffset()
                {
                    yield return new WaitForSeconds(_medalClinkConstantOffset + UnityEngine.Random.value * _medalClinkRandomOffset);
                    _medalClinking.Play(Random.value);
                }
            }
        }

        void OnChangeState(Character character, BehaviourState state)
        {
            if(state is ChasingState)
                _onNotice.Play();
        }
    }
}
