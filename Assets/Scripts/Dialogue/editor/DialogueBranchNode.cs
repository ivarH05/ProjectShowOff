using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueBranchNode : DialogueNode
    {
        internal DialogueBranchNode() : base()
        {
            nodeType = NodeType.Branch;
            title = "Branch";

            SetColor(new Color(0.35f, 0.29f, 0.2f));

            CreateInputPort("Input", inputContainer);
            CreateOutputPort("True", outputContainer);
            CreateOutputPort("False", outputContainer);
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
    }
}
