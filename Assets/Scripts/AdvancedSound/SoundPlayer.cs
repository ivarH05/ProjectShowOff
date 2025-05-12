using UnityEngine;

namespace AdvancedSound
{
    /// <summary>
    /// Plays a sound using the advanced sound system.
    /// </summary>
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] Sound _sound;
        public void Play(float volume = 1, float pitch = 1)
        {
            var res = _sound;
            res.Volume *= volume;
            res.Pitch *= pitch;
            res.Origin = transform.position;
            SoundHandler.Singleton.PlaySound(res);
        }

    }
}
