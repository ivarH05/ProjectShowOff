using System.Collections;
using UI;
using UnityEngine;

namespace Effects
{
    public class CryptCollapse : MonoBehaviour
    {
        [SerializeField] RockPummeler _pummeler;
        [SerializeField] ParticleSystem _dust;
        [SerializeField] AudioSource _caveInSound;
        [SerializeField] AnimationCurve _pummelIntensity;
        [SerializeField] AnimationCurve _dustIntensity;
        [SerializeField] float _duration = 60;
        [SerializeField] float _pummelIntensityMult = 100;
        [SerializeField] float _dustIntensityMult = 100;

        public bool DebugPlay;

        bool _playing = false;
        double _timeStart;

        public void Play()
        {
            _dust.Play();
            _playing = true;
            _timeStart = Time.timeAsDouble;
        }

        private void Update()
        {
            if(DebugPlay)
            {
                Play();
                DebugPlay = false;
            }

            if (!_playing) 
                return;

            float progress = (float)(Time.timeAsDouble - _timeStart);
            progress /= _duration;

            if(progress >= 1)
            {
                UIManager.SetState<DeathUIState>();
                _caveInSound?.Play();
                _playing = false;
                if (_pummeler != null)
                    StartCoroutine(LerpPummelerToZero());
                
                IEnumerator LerpPummelerToZero()
                {
                    float curr = _pummeler.RockFrequency;
                    float t = 0;
                    while(true)
                    {
                        t += Time.deltaTime * .3f;
                        _pummeler.RockFrequency -= curr * t;
                        if(_pummeler.RockFrequency < 0)
                        {
                            _pummeler.RockFrequency = 0;
                            break;
                        }
                        yield return null;
                    }
                }
                return;
            }

            if(_pummeler != null)
                _pummeler.RockFrequency = _pummelIntensity.Evaluate(progress) * _pummelIntensityMult;
            
            if (_dust == null) 
                return;

            var emm = _dust.emission;
            float emmStrength = _dustIntensity.Evaluate(progress) * _dustIntensityMult;
            emm.rateOverTimeMultiplier = emmStrength;
        }
    }
}
