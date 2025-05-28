using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public enum NodeType { None, Text, Branch, Start, End, Event }

    public abstract class DialogueNode : Node
    {
        public string GUID;

        public NodeType nodeType { get; internal set; }

        internal DialogueNode()
        {
            GUID = System.Guid.NewGuid().ToString();

            mainContainer.style.paddingLeft = 6;
            mainContainer.style.paddingRight = 6;

            style.minWidth = 150;
        }

        internal void SetColor(Color color)
        {
            inputContainer.style.backgroundColor = color;
            outputContainer.style.backgroundColor = color;

            color *= 0.9f;
            mainContainer.style.backgroundColor = color;
        }

        internal TextField CreateTextField(string name, VisualElement parent, EventCallback<ChangeEvent<string>> evt)
        {
            Label label = new Label(name);

            TextField textField = new TextField();
            textField.RegisterValueChangedCallback(evt);

            parent.Add(label);
            parent.Add(textField);

            return textField;
        }

        internal Label CreateLabel(string value, VisualElement parent)
        {
            if (value == "")
                return null;

            Label label = new Label(value);
            parent.Add(label);
            return label;
        }

        internal VisualElement CreateHorizontalContainer(VisualElement parent)
        {
            VisualElement horizontalContainer = new VisualElement();
            horizontalContainer.style.flexDirection = FlexDirection.Row;
            horizontalContainer.style.alignItems = Align.Center;
            parent.Add(horizontalContainer);
            return horizontalContainer;
        }

        internal Port CreateInputPort(string name, VisualElement parent)
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = name;
            parent.Add(inputPort);
            return inputPort;
        }

        internal Port CreateOutputPort(string name, VisualElement parent)
        {
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            outputPort.portName = name;
            parent.Add(outputPort);
            return outputPort;
        }

        public virtual NodeData SaveData()
        {
            return new NodeData
            {
                GUID = GUID,
                //DialogueText = "", //node is DialogueTextNode t ? t.dialogueText : 
                Position = GetPosition().position,
                //Options = options,
                type = nodeType,
            };
        }

        public virtual void LoadData(NodeData data)
        {
            this.GUID = data.GUID;
            nodeType = data.type;
        }
    }

    [System.Serializable]
    public class NodeData
    {
        public string GUID;
        public NodeType type;
        public Vector2 Position;
    }

    public class DialogueNodeOption
    {
        public VisualElement element;
        public Port port;
        public Label label;
        public string value;

        public DialogueNode node;

        public DialogueNodeOption(DialogueNode node)
        {
            this.node = node;
        }
    }
}