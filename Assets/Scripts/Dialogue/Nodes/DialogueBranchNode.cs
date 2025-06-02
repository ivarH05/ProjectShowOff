using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueBranchNode : DialogueNode
    {
        BranchData branchData;
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
            branchData = ScriptableObject.CreateInstance<BranchData>();

            var imguiContainer = new IMGUIContainer(() =>
            {
                if (branchData == null) return;

                SerializedObject so = new SerializedObject(branchData);
                so.Update();

                SerializedProperty prop = so.FindProperty("flag");
                EditorGUILayout.PropertyField(prop, GUIContent.none, true);
                so.ApplyModifiedProperties();
            });

            parent.Add(imguiContainer);

            RefreshExpandedState();
            RefreshPorts();
        }

        public override NodeData SaveData()
        {
            BranchNodeData data = new BranchNodeData(base.SaveData());
            data.flag = branchData.flag;
            return data;
        }

        public override void LoadData(NodeData data)
        {
            base.LoadData(data);
            if (!(data is BranchNodeData d))
                return;
            branchData.flag = d.flag;
        }
    }

    [System.Serializable]
    public class BranchNodeData : NodeData
    {
        public BranchNodeData(NodeData data)
        {
            this.type = data.type;
            this.GUID = data.GUID;
            this.Position = data.Position;
        }

        [SerializeReference]
        public DialogueConditionFlag flag;
    }

    [System.Serializable]
    public class BranchData : ScriptableObject
    {
        [SerializeField]
        public DialogueConditionFlag flag;
    }
}
