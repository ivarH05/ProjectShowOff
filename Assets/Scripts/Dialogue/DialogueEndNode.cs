using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueEndNode : DialogueNode
    {
        internal DialogueEndNode() : base()
        {
            nodeType = NodeType.End;
            title = "End";

            SetColor(new Color(0.32f, 0.15f, 0.18f));

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
