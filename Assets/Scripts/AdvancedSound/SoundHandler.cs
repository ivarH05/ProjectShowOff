using UnityEngine;

namespace AdvancedSound
{
    public abstract class SoundHandler : MonoBehaviour
    {
        public static SoundHandler Singleton => Instance ?? new GameObject("SoundHandler").AddComponent<DefaultSoundHandler>();
        static SoundHandler Instance;
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public abstract void PlaySound(AudioClip sound, Vector3 origin, float volume = 1, float pitch = 1);
    }
}
