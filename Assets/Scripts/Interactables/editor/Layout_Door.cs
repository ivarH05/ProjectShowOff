using Interactables;
using UnityEngine;
using UnityEditor;

namespace interactables
{
    [CustomEditor(typeof(Door))]
    public class Layout_Door : Editor
    {
        float lastAngle = 0;
        Door d;
        public override void OnInspectorGUI()
        {
            d = target as Door;

            base.OnInspectorGUI();
            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            if (d.IsLocked)
            {
                if (GUILayout.Button("Unlock"))
                {
                    Undo.RecordObject(target, "Unlocking door");
                    EditorUtility.SetDirty(target);
                    Unlock();
                }
            }
            else if (GUILayout.Button("Lock"))
            {
                Undo.RecordObject(target, "Locking door");
                EditorUtility.SetDirty(target);
                Lock();
            }

            if(d.startAngle > d.maxAngle) 
                EditorGUILayout.HelpBox("Start angle is higher than the max angle", MessageType.Warning);
        }

        /// <summary>
        /// lock the door
        /// </summary>
        private void Lock()
        {
            d.IsLocked = true;
            if (Application.isPlaying)
                return;

            // Remember the angle for potential unlocking. not saved across sessions
            lastAngle = d.startAngle;
            d.startAngle = 0;
            SceneView.RepaintAll();
        }

        private void Unlock()
        {
            d.IsLocked = false;
            if (Application.isPlaying)
                return;

            // Revert the door back to the original angle
            d.startAngle = lastAngle;
            SceneView.RepaintAll();
        }
    }
}
