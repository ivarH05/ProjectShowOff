using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueEventNode : DialogueNode
    {
        EventData eventData;
        internal DialogueEventNode() : base()
        {
            nodeType = NodeType.Event;
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
            eventData = ScriptableObject.CreateInstance<EventData>();

            var imguiContainer = new IMGUIContainer(() =>
            {
                if (eventData == null) return;

                SerializedObject so = new SerializedObject(eventData);
                so.Update();

                SerializedProperty prop = so.FindProperty("onRun");
                EditorGUILayout.PropertyField(prop, true);
                so.ApplyModifiedProperties();
            });

            parent.Add(imguiContainer);


            RefreshExpandedState();
            RefreshPorts();
        }

        public override NodeData SaveData()
        {
            EventNodeData data = new EventNodeData(base.SaveData());
            data.onRun = eventData.onRun;
            return data;
        }

        public override void LoadData(NodeData data)
        {
            base.LoadData(data);
            if (!(data is EventNodeData d))
                return;
            eventData.onRun = d.onRun;

            RefreshExpandedState();
            RefreshPorts();
        }
    }
}
