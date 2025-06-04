using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueGraphView : GraphView
    {
        public DialogueGraphView()
        {
            this.StretchToParentSize();
            SetupZoom(ContentZoomer.DefaultMinScale / 8, ContentZoomer.DefaultMaxScale * 256);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            this.RegisterCallback<MouseDownEvent>(OnContextClick);

            CreateDefaultStartNode(new Vector2(200, 250));
            CreateDefaultEndNode(new Vector2(700, 250));


            SaveLoadSystem.Load(this, "Assets/Temp/DialogueBackup.asset");
            SaveLoadSystem.ClearPath();
        }

        private void OnContextClick(MouseDownEvent evt)
        {
            if (evt.button != 1)
                return;

            Vector2 mousePosition = evt.localMousePosition;
            Vector2 worldPosition = contentViewContainer.WorldToLocal(this.LocalToWorld(mousePosition));

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Dialogue"), false, () => CreateDefaultTextNode(worldPosition));
            menu.AddItem(new GUIContent("Reroute"), false, () => CreateDefaultRerouteNode(worldPosition));
            menu.AddItem(new GUIContent("Branch"), false, () => CreateDefaultBranchNode(worldPosition));
            menu.AddItem(new GUIContent("Event"), false, () => CreateDefaultEventNode(worldPosition));
            menu.AddItem(new GUIContent("End"), false, () => CreateDefaultEndNode(worldPosition));
            menu.ShowAsContext();
        }

        public DialogueNode CreateEmptyNode(NodeType type, Vector2 position)
        {
            switch (type)
            {
                case NodeType.Text:
                    return CreateEmptyTextNode(position);
                case NodeType.Branch:
                    return CreateEmptyBranchNode(position);
                case NodeType.Start:
                    return CreateEmptyStartNode(position);
                case NodeType.End:
                    return CreateEmptyEndNode(position);
                case NodeType.Event:
                    return CreateEmptyEventNode(position);
                case NodeType.Reroute:
                    return CreateEmptyRerouteNode(position);

                default:
                    return null;
            }
        }

        public DialogueTextNode CreateEmptyTextNode(Vector2 position) => SetupNode(DialogueTextNode.EmptyNode(), position);
        public DialogueTextNode CreateDefaultTextNode(Vector2 position) => SetupNode(DialogueTextNode.DefaultNode(), position);

        public DialogueBranchNode CreateEmptyBranchNode(Vector2 position) => SetupNode(DialogueBranchNode.EmptyNode(), position);
        public DialogueBranchNode CreateDefaultBranchNode(Vector2 position) => SetupNode(DialogueBranchNode.DefaultNode(), position);

        public DialogueStartNode CreateEmptyStartNode(Vector2 position) => SetupNode(DialogueStartNode.EmptyNode(), position);
        public DialogueStartNode CreateDefaultStartNode(Vector2 position) => SetupNode(DialogueStartNode.DefaultNode(), position);

        public DialogueEndNode CreateEmptyEndNode(Vector2 position) => SetupNode(DialogueEndNode.EmptyNode(), position);
        public DialogueEndNode CreateDefaultEndNode(Vector2 position) => SetupNode(DialogueEndNode.DefaultNode(), position);

        public DialogueEventNode CreateEmptyEventNode(Vector2 position) => SetupNode(DialogueEventNode.EmptyNode(), position);
        public DialogueEventNode CreateDefaultEventNode(Vector2 position) => SetupNode(DialogueEventNode.DefaultNode(), position);

        public DialogueRerouteNode CreateEmptyRerouteNode(Vector2 position) => SetupNode(DialogueRerouteNode.EmptyNode(), position);
        public DialogueRerouteNode CreateDefaultRerouteNode(Vector2 position) => SetupNode(DialogueRerouteNode.DefaultNode(), position);


        private T SetupNode<T>(T node, Vector2 position) where T : DialogueNode
        {
            node.SetPosition(new Rect(position, new Vector2(200, 150)));
            AddElement(node);
            return node;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach((port) =>
            {
                if (startPort == port)
                    return;

                if (startPort.node == port.node)
                    return;

                if (startPort.direction != port.direction && startPort.portType == port.portType)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        public void ResetGraph(bool prompt = false)
        {
            if (prompt)
            {
                int option = EditorUtility.DisplayDialogComplex(
                    "Unsaved Changes",
                    "You have unsaved changes. What would you like to do?",
                    "Save",        // option 0
                    "Don't Save",  // option 1
                    "Cancel"       // option 2
                );
                switch (option)
                {
                    case 0:
                        SaveLoadSystem.Save(this);
                        break;

                    case 1:
                        break;

                    case 2:
                        return;
                }
            }
            ClearGraph();
            SaveLoadSystem.ClearPath();

            CreateDefaultStartNode(new Vector2(200, 250));
            CreateDefaultEndNode(new Vector2(700, 250));
        }

        public void ClearGraph()
        {
            foreach (var edge in edges.ToList())
            {
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                RemoveElement(edge);
            }

            foreach (var node in nodes.ToList())
            {
                RemoveElement(node);
            }
        }

        public override EventPropagation DeleteSelection()
        {
            var edgesToDelete = new List<Edge>();

            foreach (var selectedElement in selection)
            {
                if (selectedElement is Node node)
                    edgesToDelete.AddRange(node.Query<Port>().Build().SelectMany(port => port.connections));
            }

            foreach (var edge in edgesToDelete.Distinct())
            {
                edge.input?.Disconnect(edge);
                edge.output?.Disconnect(edge);
                RemoveElement(edge);
            }

            return base.DeleteSelection();
        }
        protected override bool canDeleteSelection
        {
            get
            {
                foreach (var selected in selection)
                {
                    if (selected is DialogueStartNode)
                        return false;
                }
                return base.canDeleteSelection;
            }
        }

        public void Save()
        {
            SaveLoadSystem.Save(this);
        }
    }
}

