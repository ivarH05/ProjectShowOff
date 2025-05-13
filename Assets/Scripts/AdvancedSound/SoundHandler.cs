using System.Collections;
using UnityEngine;

namespace AdvancedSound
{
    public class SoundHandler : MonoBehaviour
    {
        public static SoundHandler Singleton => Instance ?? new GameObject("SoundHandler").AddComponent<SoundHandler>();
        static SoundHandler Instance;
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PlaySound(Sound snd)
        {
            var GO = new GameObject("Temp sound");
            GO.transform.position = snd.Origin;
            var player = GO.AddComponent<AudioSource>();
            player.clip = snd.Clip;
            player.volume = snd.Volume;
            player.pitch = snd.Pitch;
            player.maxDistance = snd.Range;
            player.rolloffMode = AudioRolloffMode.Custom;
            player.SetCustomCurve(AudioSourceCurveType.SpatialBlend, new(new Keyframe(0, 1, 0, 0)));
            AnimationCurve audioFalloff = new(
                new(0, 1),
                new(snd.LoudThreshold, .5f),
                new(snd.ModerateThreshold, .15f),
                new(1, 0)
            );
            audioFalloff.SmoothTangents(1, 0);
            audioFalloff.SmoothTangents(2, 0);
            player.SetCustomCurve(AudioSourceCurveType.CustomRolloff, audioFalloff);
            player.Play();

            StartCoroutine(DestroyIfStopped());
            IEnumerator DestroyIfStopped()

            {
                while (player.isPlaying)
                    yield return new WaitForFixedUpdate();
                Destroy(GO);
            }
        }
    }
}
