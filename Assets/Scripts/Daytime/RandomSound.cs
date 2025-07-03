using System.Collections;
using UnityEngine;

namespace Daytime
{
    /// <summary>
    /// Plays a random sound every so often.
    /// </summary>
    public class RandomSound : MonoBehaviour
    {
        [SerializeField] AudioSource[] _sounds;
        [SerializeField] float _minimumDelay;
        [SerializeField] float _maximumDelay;
        int _previousSoundIndex;

        private void Start()
        {
            if(_sounds == null || _sounds.Length == 0)
            {
                Debug.LogError("Cant play random sound - there are no sounds assigned.", this);
                return;
            }
            if(_maximumDelay < _minimumDelay)
            {
                Debug.LogError("Cant play randomm sound - the maximum delay is smaller than the minimum delay.");
                return;
            }

            StartCoroutine(PlayRandomSound(Random.value));
        }

        IEnumerator PlayRandomSound(float delayMult = 1)
        {
            yield return new WaitForSeconds(Random.Range(_minimumDelay, _maximumDelay) * delayMult);

            int attempt = default;
            for(int i = 0; i < 10; i++)
            {
                attempt = Random.Range(0, _sounds.Length);
                if (attempt != _previousSoundIndex)
                    break;
            }

            _previousSoundIndex = attempt;
            _sounds[attempt].Play();

            StartCoroutine(PlayRandomSound());
        }
    }
}
