using System.Collections;
using UnityEngine;

namespace AdvancedSound
{
    public class DefaultSoundHandler : SoundHandler
    {
        public override void PlaySound(AudioClip sound, Vector3 origin, float volume = 1, float pitch = 1)
        {
            var GO = new GameObject("Temp sound");
            GO.transform.position = origin;
            var player = GO.AddComponent<AudioSource>();
            player.volume = volume;
            player.pitch = pitch;
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
