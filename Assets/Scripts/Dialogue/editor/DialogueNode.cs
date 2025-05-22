using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueNode : Node
    {
        public string GUID;

        public string dialogueText { get; private set; } = "";
        public List<DialogueNodeOption> options = new List<DialogueNodeOption>();

        [SerializeField]
        private VisualElement _AddItemButton;

        private TextField _dialogueTextField;

        public DialogueNode()
        {
            mainContainer.style.paddingLeft = 6;
            mainContainer.style.paddingRight = 6;

            SetColor(new Color(0.26f, 0.26f, 0.26f));
            CreateInputFields(inputContainer);
            CreateOutputOption(mainContainer, "Continue");

            // Refresh UI to show changes
            RefreshExpandedState();
            RefreshPorts();
        }

        public void SetText(string text)
        {
            if(_dialogueTextField != null)
                _dialogueTextField.value = text;
            dialogueText = text;
        }

        public void CreateOutputOption(string value)
        {
            CreateOutputOption(mainContainer, value);
        }

        void RemoveNodeOption(DialogueNodeOption option)
        {
            foreach (var edge in option.port.connections.ToList())
            {
                edge.input?.Disconnect(edge);
                edge.output?.Disconnect(edge);

                edge.RemoveFromHierarchy();
            }

            option.port.DisconnectAll();
            option.element.RemoveFromHierarchy();
            options.Remove(option);
            for (int i = 0; i < options.Count; i++)
            {
                DialogueNodeOption o = options[i];
                o.label.text = $"Option {i}";
            }
        }

        void CreateInputFields(VisualElement parent)
        {
            VisualElement horizontalContainer = CreateHorizontalContainer(parent);
            CreateInputNode("", horizontalContainer);
            CreateLabel("Input", horizontalContainer);

            TextField textField = CreateTextField(dialogueText, parent, evt => { dialogueText = evt.newValue; });
            textField.multiline = true;
            textField.style.minWidth = 150;
            textField.style.maxWidth = 350;
        }

        void CreateOutputOption(VisualElement parent, string value = "")
        {
            int index = options.Count;
            DialogueNodeOption option = new DialogueNodeOption(this);
            VisualElement container = new VisualElement();
            parent.Add(container);

            VisualElement labelContainer = CreateHorizontalContainer(container);
            labelContainer.style.alignItems = Align.FlexStart;
            labelContainer.style.justifyContent = Justify.FlexStart;
            Label label = CreateLabel($"option {index}", labelContainer);

            Button removeButton = new Button(() => { RemoveNodeOption(option); }) { text = "-" };
            removeButton.style.width = 20;
            labelContainer.Add(removeButton);


            VisualElement valueContainer = CreateHorizontalContainer(container);
            valueContainer.style.alignItems = Align.FlexEnd;
            valueContainer.style.justifyContent = Justify.FlexEnd;

            TextField textField = CreateTextField("", valueContainer, evt => { options[index].value = evt.newValue; });
            textField.value = value;
            textField.style.maxWidth = 330;
            textField.style.minWidth = 130;
            Port OutputPort = CreateOutputNode("", valueContainer);

            ResetAddItemButtion();


            option.element = container;
            option.label = label;
            option.port = OutputPort;
            options.Add(option);
        }

        void ResetAddItemButtion()
        {
            if (_AddItemButton != null)
                _AddItemButton.RemoveFromHierarchy();

            Button addItemButton = new Button(() => { CreateOutputOption(mainContainer); }) { text = "Add Option" };
            _AddItemButton = addItemButton;
            mainContainer.Add(addItemButton);
        }

        void SetColor(Color color)
        {
            inputContainer.style.backgroundColor = color;
            outputContainer.style.backgroundColor = color;

            color *= 0.9f;
            mainContainer.style.backgroundColor = color;
        }

        TextField CreateTextField(string name, VisualElement parent, EventCallback<ChangeEvent<string>> evt)
        {
            Label label = new Label(name);

            TextField textField = new TextField();
            textField.RegisterValueChangedCallback(evt);

            parent.Add(label);
            parent.Add(textField);

            return textField;
        }

        Label CreateLabel(string value, VisualElement parent)
        {
            if (value == "")
                return null;

            Label label = new Label(value);
            parent.Add(label);
            return label;
        }

        VisualElement CreateHorizontalContainer(VisualElement parent)
        {
            VisualElement horizontalContainer = new VisualElement();
            horizontalContainer.style.flexDirection = FlexDirection.Row;
            horizontalContainer.style.alignItems = Align.Center;
            parent.Add(horizontalContainer);
            return horizontalContainer;
        }

        Port CreateInputNode(string name, VisualElement parent)
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = name;
            parent.Add(inputPort);
            return inputPort;
        }

        Port CreateOutputNode(string name, VisualElement parent)
        {
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            outputPort.portName = name;
            parent.Add(outputPort);
            return outputPort;
        }
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