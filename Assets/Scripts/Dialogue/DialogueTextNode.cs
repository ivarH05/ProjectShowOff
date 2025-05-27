using DialogueSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueTextNode : DialogueNode
    {
        [SerializeField]
        private VisualElement _AddItemButton;
        private TextField _dialogueTextField;
        public string dialogueText { get; private set; } = "";
        public List<DialogueNodeOption> options = new List<DialogueNodeOption>();

        internal DialogueTextNode() : base()
        {
            nodeType = NodeType.Text;
            title = "Dialogue Node";

            SetColor(new Color(0.26f, 0.26f, 0.26f));
            CreateInputFields(inputContainer);
        }

        public static DialogueTextNode DefaultNode(string defaultText = "")
        {
            DialogueTextNode node = new DialogueTextNode();
            node.SetText(defaultText);
            node.CreateOutputOption(node.mainContainer, "Continue");
            return node;

        }

        public static DialogueTextNode EmptyNode(string defaultText = "")
        {
            DialogueTextNode node = new DialogueTextNode();
            node.SetText(defaultText);
            return node;
        }

        public void SetText(string text)
        {
            _dialogueTextField.value = text;
            dialogueText = text;
        }


        internal void RemoveNodeOption(DialogueNodeOption option)
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

        internal void CreateInputFields(VisualElement parent)
        {
            VisualElement horizontalContainer = CreateHorizontalContainer(parent);
            CreateInputPort("", horizontalContainer);
            CreateLabel("Input", horizontalContainer);

            TextField textField = CreateTextField(dialogueText, parent, evt => { dialogueText = evt.newValue; });
            textField.multiline = true;
            textField.style.minWidth = 150;
            textField.style.maxWidth = 350;
            _dialogueTextField = textField;
        }

        public void CreateOutputOption(string value) => CreateOutputOption(mainContainer, value);
        internal void CreateOutputOption(VisualElement parent, string value = "")
        {
            int index = options.Count;
            DialogueNodeOption option = new DialogueNodeOption(this) { value = value };
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
            Port OutputPort = CreateOutputPort("", valueContainer);

            ResetAddItemButtion();


            option.element = container;
            option.label = label;
            option.port = OutputPort;
            options.Add(option);
        }

        internal void ResetAddItemButtion()
        {
            if (_AddItemButton != null)
                _AddItemButton.RemoveFromHierarchy();

            Button addItemButton = new Button(() => { CreateOutputOption(mainContainer); }) { text = "Add Option" };
            _AddItemButton = addItemButton;
            mainContainer.Add(addItemButton);
        }
    }
}
