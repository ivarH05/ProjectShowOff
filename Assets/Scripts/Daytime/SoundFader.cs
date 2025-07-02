using System.Collections;
using UnityEngine;

namespace Daytime
{
    public class SoundFader : MonoBehaviour
    {
        [SerializeField] AudioSource _source;
        [SerializeField] float _fadeSpeedSeconds = 1;
        public void FadeToValue(float value)
        {
            StartCoroutine(Fader(value));
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }

        IEnumerator Fader(float value)
        {
            float start = _source.volume;
            float cur = _source.volume;
            float time = 0;
            while (cur == _source.volume)
            {
                time += Time.deltaTime;
                _source.volume = Mathf.Lerp(start, value, time / _fadeSpeedSeconds);
                if (_source.volume <= 0)
                {
                    _source.volume = 0;
                    break;
                }
                if (_source.volume >= 1)
                {
                    _source.volume = 1;
                    break;
                }
                cur = _source.volume;
                yield return null;
            }
        }
    }
}
