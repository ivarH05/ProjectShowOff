using UnityEngine;

namespace AdvancedSound
{
    /// <summary>
    /// Plays a sound using the advanced sound system.
    /// </summary>
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] AudioClip _sound;
        public void Play(float volume = 1, float pitch = 1)
        {
            SoundHandler.Singleton.PlaySound(_sound, transform.position, volume, pitch);
        }

    }
}
