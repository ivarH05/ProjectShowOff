using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueEndNode : DialogueNode
    {
        internal DialogueEndNode() : base()
        {
            nodeType = NodeType.End;
            title = "Dialogue Node";

            SetColor(new Color(0.3f, 0.2f, 0.2f));

            CreateInputPort("End", inputContainer);
        }

        public static DialogueEndNode DefaultNode()
        {
            DialogueEndNode node = new DialogueEndNode();
            return node;

        }

        public static DialogueEndNode EmptyNode()
        {
            DialogueEndNode node = new DialogueEndNode();
            return node;
        }
    }
}
