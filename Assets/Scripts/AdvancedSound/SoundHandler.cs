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

        public void PlaySound(Sound snd, Vector3 position, float volumeMultiplier = 1, float pitchMultiplier = 1)
        {
            var GO = new GameObject("Temp sound");
            GO.transform.position = position;
            var player = GO.AddComponent<AudioSource>();
            player.clip = snd.GetClip();
            player.volume = snd.Volume * volumeMultiplier;
            player.pitch = snd.Pitch * pitchMultiplier;
            player.maxDistance = snd.AudibleRange;
            player.rolloffMode = AudioRolloffMode.Custom;
            player.SetCustomCurve(AudioSourceCurveType.SpatialBlend, new(new Keyframe(0, 1, 0, 0)));
            AnimationCurve audioFalloff = new(
                new(0, 1),
                new(snd.LoudThreshold, .5f),
                new(snd.ModerateThreshold, .15f),
                new(snd.FaintThreshold, .07f),
                new(1f, 0)
            );
            audioFalloff.SmoothTangents(1, 0);
            audioFalloff.SmoothTangents(2, 0);
            audioFalloff.SmoothTangents(3, 0);
            player.SetCustomCurve(AudioSourceCurveType.CustomRolloff, audioFalloff);
            player.Play();

            foreach(var listener in SoundListener.Listeners)
            {
                var dist = (listener.transform.position - position).sqrMagnitude;
                if (dist > snd.AudibleRange * snd.AudibleRange * volumeMultiplier) continue;
                float normDist = Mathf.Sqrt(dist) / (snd.AudibleRange * volumeMultiplier);
                HeardSound res = default;
                res.Origin = position;
                res.Type = snd.Type;
                res.Loudness = Loudness.Faint;
                if(normDist < snd.ModerateThreshold)
                    res.Loudness = normDist < snd.LoudThreshold ? Loudness.Loud : Loudness.Moderate;
                if(res.Type == SoundType.Ambient) res.Loudness = Loudness.Inaudible;
                listener.OnHearSound(res);
            }

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
