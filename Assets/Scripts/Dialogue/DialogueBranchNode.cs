using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueBranchNode : DialogueNode
    {
        BranchData data;
        internal DialogueBranchNode() : base()
        {
            nodeType = NodeType.Branch;
            title = "Branch";

            SetColor(new Color(0.4f, 0.27f, 0.15f));
            style.minWidth = 100;
            style.width = 150;

            CreateInputPort("Input", inputContainer);
            CreateOutputPort("True", outputContainer);
            CreateOutputPort("False", outputContainer);

            CreateBranchSettings(extensionContainer);
        }

        public static DialogueBranchNode DefaultNode()
        {
            DialogueBranchNode node = new DialogueBranchNode();
            return node;

        }

        public static DialogueBranchNode EmptyNode()
        {
            DialogueBranchNode node = new DialogueBranchNode();
            return node;
        }
        internal void CreateBranchSettings(VisualElement parent)
        {
            data = ScriptableObject.CreateInstance<BranchData>();

            var imguiContainer = new IMGUIContainer(() =>
            {
                if (data == null) return;

                SerializedObject so = new SerializedObject(data);
                so.Update();

                SerializedProperty prop = so.FindProperty("flag");
                EditorGUILayout.PropertyField(prop, GUIContent.none, true);
                so.ApplyModifiedProperties();
            });

            parent.Add(imguiContainer);

            RefreshExpandedState();
            RefreshPorts();
        }
    }

    [System.Serializable]
    public class BranchData : ScriptableObject
    {
        [SerializeField]
        public DialogueConditionFlag flag;
    }
}
