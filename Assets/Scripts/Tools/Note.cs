using UnityEditor;
using UnityEngine;

namespace Tools
{
    class Note : MonoBehaviour
    {
#if UNITY_EDITOR
        public string Text;

        [CustomEditor(typeof(Note))]
        public class NoteEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                var t = target as Note;
                EditorGUI.BeginChangeCheck();
                var newtxt = EditorGUILayout.TextArea(t.Text);
                if(EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(t, "Change note");
                    t.Text = newtxt;
                    EditorUtility.SetDirty(t);
                }
            }
        }
#else
        void Start()
        {
            Destroy(this);
        }
#endif
    }
}