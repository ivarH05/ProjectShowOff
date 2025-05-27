using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueEventNode : DialogueNode
    {
        EventData data;
        internal DialogueEventNode() : base()
        {
            nodeType = NodeType.Branch;
            title = "Event";

            SetColor(new Color(0.35f, 0.17f, 0.27f));
            style.minWidth = 225;

            CreateInputPort("Input", inputContainer);
            CreateOutputPort("Continue", outputContainer);
            CreateEventSettings(extensionContainer);
        }

        public static DialogueEventNode DefaultNode()
        {
            DialogueEventNode node = new DialogueEventNode();
            return node;

        }

        public static DialogueEventNode EmptyNode()
        {
            DialogueEventNode node = new DialogueEventNode();
            return node;
        }
        internal void CreateEventSettings(VisualElement parent)
        {
            data = ScriptableObject.CreateInstance<EventData>();

            var imguiContainer = new IMGUIContainer(() =>
            {
                if (data == null) return;

                SerializedObject so = new SerializedObject(data);
                so.Update();

                SerializedProperty prop = so.FindProperty("onRun");
                EditorGUILayout.PropertyField(prop, true);
                so.ApplyModifiedProperties();
            });

            parent.Add(imguiContainer);


            RefreshExpandedState();
            RefreshPorts();
        }
    }

    [System.Serializable]
    public class EventData : ScriptableObject
    {
        [SerializeField]
        public DialogueEvent onRun = new DialogueEvent();
    }
}
