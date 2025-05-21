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
            if(_sound == null)
            {
                Debug.LogError("Sound was null. Set an existing one, or create a new one using the create menu.", this);
            }
            var res = _sound;
            SoundHandler.Singleton.PlaySound(res, transform.position, volume, pitch);
        }

#if UNITY_EDITOR
        [SerializeField] bool _showGizmos = true;
        private void OnDrawGizmosSelected()
        {
            if(!_showGizmos) return;
            if (_sound == null) return;
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

                if (snd._sound == null) return;

                GUILayout.Label("Sound settings:");
                EditorGUI.BeginChangeCheck();

                var audRange = EditorGUILayout.FloatField("Audible range:", snd._sound.AudibleRange);
                var faintThr = snd._sound.FaintThreshold; OptionalSlider(ref faintThr, 0, 1, "Faint threshold:");
                var moderateThr = snd._sound.ModerateThreshold; OptionalSlider(ref moderateThr, 0, 1, "Moderate threshold:");
                var loudThr = snd._sound.LoudThreshold; OptionalSlider(ref loudThr, 0, 1, "Loud threshold:");

                void OptionalSlider(ref float value, float minValue, float maxValue, string name)
                {
                    GUILayout.BeginHorizontal();
                    value = EditorGUILayout.FloatField(name, value);
                    value = GUILayout.HorizontalSlider(value, minValue, maxValue, GUILayout.MinWidth(150));
                    GUILayout.EndHorizontal();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(snd._sound, "Change sound settings");
                    snd._sound.AudibleRange = audRange;
                    snd._sound.FaintThreshold = Mathf.Min(faintThr, 1);
                    snd._sound.ModerateThreshold = Mathf.Min(moderateThr, 1);
                    snd._sound.LoudThreshold = Mathf.Min(loudThr, 1);
                    EditorUtility.SetDirty(snd._sound);
                    SceneView.RepaintAll();
                }
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
