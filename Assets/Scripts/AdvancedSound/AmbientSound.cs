using System;
using System.Collections;
using UnityEngine;
using Crypts;

namespace AdvancedSound
{
    public class AmbientSound : MonoBehaviour
    {
        [SerializeField] Sound _ambientSounds;
        [SerializeField] float _minimumSoundDelay = .1f;
        [SerializeField] float _maximumSoundDelay = 1f;
        [SerializeField] float _minimumDistance = 5;
        [SerializeField] float _maximumDistance = 40;

        private void Start()
        {
            if (_ambientSounds != null)
                StartCoroutine(SoundChain());
            else Debug.LogError("Ambient sounds were null.", this);

            IEnumerator SoundChain()
            {
                while(true)
                {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(_minimumSoundDelay, _maximumSoundDelay));
                    RandomSound();
                }
            }
        }

        void RandomSound()
        {
            var dist = UnityEngine.Random.Range(_minimumDistance, _maximumDistance);
            float direction = UnityEngine.Random.Range(0, 360);
            var p = Crypt.GetClosestPoint(transform.position + new Vector3(MathF.Cos(direction) * dist, 0, MathF.Sin(direction) * dist));
            SoundHandler.Singleton.PlaySound(_ambientSounds, p);
        }
    }
}