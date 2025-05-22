using System.Collections.Generic;
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
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            this.RegisterCallback<MouseDownEvent>(OnContextClick);
        }

        private void OnContextClick(MouseDownEvent evt)
        {
            if (evt.button != 1)
                return;

            Vector2 mousePosition = evt.localMousePosition;
            Vector2 worldPosition = contentViewContainer.WorldToLocal(this.LocalToWorld(mousePosition));

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Dialogue Node"), false, () => CreateNode("Dialogue Node", worldPosition));
            menu.ShowAsContext();
        }

        public DialogueNode CreateNode(string nodeName, Vector2 position)
        {
            DialogueNode node = new DialogueNode
            {
                title = nodeName,
                GUID = System.Guid.NewGuid().ToString(),

            };

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
    }
}

