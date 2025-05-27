using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueStartNode : DialogueNode
    {
        internal DialogueStartNode() : base()
        {
            nodeType = NodeType.Start;
            title = "Start";

            SetColor(new Color(0.17f, 0.3f, 0.17f));

            CreateOutputPort("Start", outputContainer);
        }

        public static DialogueStartNode DefaultNode()
        {
            DialogueStartNode node = new DialogueStartNode();
            return node;

        }

        public static DialogueStartNode EmptyNode()
        {
            DialogueStartNode node = new DialogueStartNode();
            return node;
        }
    }
}
