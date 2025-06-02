using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueRerouteNode : DialogueNode
    {
        internal DialogueRerouteNode() : base()
        {
            nodeType = NodeType.Reroute;
            title = "";

            CreateInputPort("", inputContainer);
            CreateOutputPort("", outputContainer);
        }

        public static DialogueRerouteNode DefaultNode()
        {
            DialogueRerouteNode node = new DialogueRerouteNode();
            return node;

        }

        public static DialogueRerouteNode EmptyNode()
        {
            DialogueRerouteNode node = new DialogueRerouteNode();
            return node;
        }
    }
}

