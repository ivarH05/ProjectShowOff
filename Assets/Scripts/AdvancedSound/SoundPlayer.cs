using UnityEditor;
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

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _sound.Range);
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, _sound.Range * _sound.ModerateThreshold);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _sound.Range * _sound.LoudThreshold);
        }

        [CustomEditor(typeof(SoundPlayer))]
        class SoundPlayerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                var snd = target as SoundPlayer;

                GUI.enabled = Application.isPlaying;
                if (GUILayout.Button("Play sound (play mode only)"))
                    snd.Play();
                GUI.enabled = true;

                GUI.color = new(1,.3f,0);
                if (snd._sound.Range <= 0
                    || snd._sound.ModerateThreshold == 0
                    || snd._sound.LoudThreshold == 0)
                EditorGUILayout.LabelField("Thresholds/range is 0! The SoundPlayer may not work correctly.");
                GUI.color = Color.white;
                if(snd._sound.ModerateThreshold < snd._sound.LoudThreshold)
                    snd._sound.ModerateThreshold = snd._sound.LoudThreshold;
            }
        }
#endif
    }
}
