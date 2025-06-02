using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Crypts
{
    [CustomEditor(typeof(Crypt))]
    public class Editor_Crypt : Editor
    {
        public override void OnInspectorGUI()
        {
            Crypt c = (Crypt)target;

            base.OnInspectorGUI();

            if (c.isEditing)
            {
                if (GUILayout.Button("Done"))
                    StopEdit(c);
            }
            else if (GUILayout.Button("Edit"))
                StartEdit(c);
        }
        void OnSceneGUI()
        {
            Crypt c = (Crypt)target;
            if (!c.isEditing)
                return;

            Event e = Event.current;
            if (e.IsRightMouseButton())
                return;

            if (e.type == EventType.MouseDown)
                c.EditorMouseDown(e.mousePosition);
            if (e.type == EventType.MouseDrag)
                c.EditorMouse(e.mousePosition);
            if (e.type == EventType.MouseUp)
                c.EditorMouseUp(e.mousePosition);
        }

        void StartEdit(Crypt c)
        {
            EditorMouse.Lock(c.gameObject);
            c.isEditing = true;
        }

        void StopEdit(Crypt c)
        {
            EditorMouse.Unluck();
            c.isEditing = false;
        }
    }
}