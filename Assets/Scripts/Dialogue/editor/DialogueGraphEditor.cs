using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueGraphEditor : EditorWindow
    {
        private DialogueGraphView graphView;

        [MenuItem("Window/Dialogue Graph")]
        public static void OpenDialogueGraphEditor()
        {
            DialogueGraphEditor window = GetWindow<DialogueGraphEditor>("Dialogue Graph Editor");
            window.Show();
        }

        public void CreateGUI()
        {
            // You can replace this with your actual graph view initialization
            graphView = new DialogueGraphView
            {
                name = "Dialogue Graph"
            };
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);

            AssemblyReloadEvents.beforeAssemblyReload += () => { SaveLoadSystem.Save(graphView, "Assets/Temp/DialogueBackup.asset"); };

            // Create a toolbar with Save and Load buttons
            var toolbar = new Toolbar();

            var saveButton = new Button(() => SaveLoadSystem.Save(graphView))
            {
                text = "Save"
            };

            var saveAsButton = new Button(() => SaveLoadSystem.SaveAs(graphView))
            {
                text = "Save as"
            };

            var loadButton = new Button(() => SaveLoadSystem.Load(graphView))
            {
                text = "Load"
            };
            var ResetButton = new Button(() => graphView.ResetGraph(true))
            {
                text = "Reset"
            };

            toolbar.Add(saveButton);
            toolbar.Add(saveAsButton);
            toolbar.Add(loadButton);
            toolbar.Add(ResetButton);

            rootVisualElement.Add(toolbar);
        }
        void OnGUI()
        {
            Event e = Event.current;

            if ((e.control || e.command) && e.type == EventType.KeyDown && e.keyCode == KeyCode.S)
            {
                e.Use();
                SaveLoadSystem.Save(graphView);
            }
        }
    }
}