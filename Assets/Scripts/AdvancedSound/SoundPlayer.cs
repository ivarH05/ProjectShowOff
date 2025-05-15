using System;
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
            res.Pitch *= pitch;
            SoundHandler.Singleton.PlaySound(res, transform.position, volume);
        }

#if UNITY_EDITOR
        [SerializeField] bool _showGizmos = true;
        private void OnDrawGizmosSelected()
        {
            if(!_showGizmos) return;
            Gizmos.color = new(1, 1, 1, .25f);
            Gizmos.DrawWireSphere(transform.position, _sound.AudibleRange);
            Gizmos.color = new(1,1,0,.35f);
            Gizmos.DrawWireSphere(transform.position, _sound.AudibleRange * _sound.FaintThreshold);
            Gizmos.color = new(1,.5f,0,.5f);
            Gizmos.DrawWireSphere(transform.position, _sound.AudibleRange * _sound.ModerateThreshold);
            Gizmos.color = new(1,0,0,.5f);
            Gizmos.DrawWireSphere(transform.position, _sound.AudibleRange * _sound.LoudThreshold);
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
                if (snd._sound.AudibleRange <= 0
                    || snd._sound.FaintThreshold == 0
                    || snd._sound.ModerateThreshold == 0
                    || snd._sound.LoudThreshold == 0)
                EditorGUILayout.LabelField("Thresholds/range is 0! The SoundPlayer may not work correctly.");
                GUI.color = Color.white;
                if(snd._sound.FaintThreshold < snd._sound.ModerateThreshold)
                    snd._sound.FaintThreshold = snd._sound.ModerateThreshold;
                if(snd._sound.ModerateThreshold < snd._sound.LoudThreshold)
                    snd._sound.ModerateThreshold = snd._sound.LoudThreshold;
            }
        }
#endif
    }
}
