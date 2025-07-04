using System.Collections;
using AdvancedSound;
using UI;
using UnityEngine;

namespace Effects
{
    public class CryptCollapse : MonoBehaviour
    {
        [SerializeField] RockPummeler _pummeler;
        [SerializeField] ParticleSystem _dust;
        [SerializeField] ScreenShake _screenShake;
        [SerializeField] AnimationCurve _pummelIntensity;
        [SerializeField] AnimationCurve _dustIntensity;
        [SerializeField] AnimationCurve _screenShakeIntensity;
        [SerializeField] AudioSource _caveInSound;
        [SerializeField] Sound _dustSound;
        [SerializeField] float _duration = 60;
        [SerializeField] float _pummelIntensityMult = 100;
        [SerializeField] float _dustIntensityMult = 100;
        [SerializeField] float _screenShakeIntensityMult = 1;
        [SerializeField] float _dustSoundFrequency = 1;

        public bool DebugPlay;

        bool _playing = false;
        double _timeStart;
        float _dustCooldown;

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
                        _pummeler.RockFrequency = curr * (1-t);
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

            float dustStrength = _dustIntensity.Evaluate(progress) * _dustIntensityMult;
            _dustCooldown += Time.deltaTime * _dustSoundFrequency * dustStrength / _dustIntensityMult; ;
            if (_dustCooldown > 1 && _dustSound != null)
            {
                _dustCooldown = 0;
                var off = Random.insideUnitCircle * 10;
                SoundHandler.Singleton.PlaySound(_dustSound, transform.position + new Vector3(off.x, 2, off.y), 1, 1);
            }

            if (_screenShake != null)
                _screenShake.Strength = _screenShakeIntensity.Evaluate(progress) * _screenShakeIntensityMult;

            if(_pummeler != null)
                _pummeler.RockFrequency = _pummelIntensity.Evaluate(progress) * _pummelIntensityMult;
            
            if (_dust == null) 
                return;

            var emm = _dust.emission;
            emm.rateOverTimeMultiplier = dustStrength;
        }
    }
}
