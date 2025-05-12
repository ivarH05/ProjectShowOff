using System.Collections;
using UnityEngine;

namespace AdvancedSound
{
    public class DefaultSoundHandler : SoundHandler
    {
        public override void PlaySound(Sound snd)
        {
            var GO = new GameObject("Temp sound");
            GO.transform.position = snd.Origin;
            var player = GO.AddComponent<AudioSource>();
            player.volume = snd.Volume;
            player.pitch = snd.Pitch;
            player.SetCustomCurve(AudioSourceCurveType.SpatialBlend, new(new Keyframe(0, 1, 0, 0)));
            player.Play();

            StartCoroutine(DestroyIfStopped());
            IEnumerator DestroyIfStopped()
            {
                while(player.isPlaying)
                    yield return new WaitForFixedUpdate();
                Destroy(GO);
            }
        }
    }
}
