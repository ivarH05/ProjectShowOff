using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.StandaloneInputModule;


namespace DialogueSystem
{
    public static class SaveLoadSystem
    {

        public static void Save(DialogueGraphView graphView)
        {
            string path = EditorUtility.SaveFilePanel("Save Dialogue Graph", "Assets", "DialogueGraph", "asset");
            if (string.IsNullOrEmpty(path))
                return;

            path = "Assets" + path.Substring(Application.dataPath.Length);
            Save(graphView, path);
        }
        public static void Save(DialogueGraphView graphView, string path)
        {
            Dialogue saveData = ScriptableObject.CreateInstance<Dialogue>();

            foreach (var node in graphView.nodes.OfType<DialogueNode>())
            {
                NodeData nodeData = node.SaveData();

                if (node is DialogueTextNode textNode)
                    ((TextNodeData)nodeData).Options = ExtractOptions(textNode);

                saveData.Connections.AddRange(ExtractConnections(node));

                saveData.Nodes.Add(nodeData);
            }

            AssetDatabase.CreateAsset(saveData, path);
        }

        private static List<ConnectionData> ExtractConnections(DialogueNode node)
        {
            List<ConnectionData> result = new List<ConnectionData>();

            List<Port> ports = node.mainContainer.Query<Port>().Build().ToList();

            for (int i = 0; i < ports.Count; i++)
            {
                Port port = ports[i];
                if (port.direction == Direction.Input)
                    continue;

                var connections = port.connections.ToArray();
                for (int j = 0; j < connections.Length; j++)
                {
                    var connection = connections[j];
                    var inputNode = FindParentOfType<DialogueNode>(connection.input);

                    result.Add(new ConnectionData
                    {
                        OutputNodeGUID = node.GUID,
                        OutputNodeIndex = i,
                        InputNodeGUID = inputNode.GUID
                    });
                }
            }

            return result;
        }

        private static List<OptionData> ExtractOptions(DialogueTextNode node)
        {
            List<OptionData> result = new List<OptionData>();

            var oldOptions = node.options;
            for (int i = 0; i < oldOptions.Count; i++)
            {
                var option = oldOptions[i];
                result.Add(new OptionData()
                {
                    index = i,
                    ResponseText = option.value,
                });
            }

            return result;
        }


        public static void Load(DialogueGraphView graphView)
        {
            string path = EditorUtility.OpenFilePanel("Load Dialogue Graph", "Assets", "asset");
            if (string.IsNullOrEmpty(path))
                return;
            path = "Assets" + path.Substring(Application.dataPath.Length);

            Load(graphView, path);
        }


        public static void Load(DialogueGraphView graphView, string path)
        {
            if (!File.Exists(path)) 
                return;

            graphView.ClearGraph();

            var saveData = AssetDatabase.LoadAssetAtPath<Dialogue>(path);

            Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();
            foreach (var nodeData in saveData.Nodes)
            {
                DialogueNode node = graphView.CreateEmptyNode(nodeData.type, nodeData.Position);
                node.LoadData(nodeData);

                nodeLookup.Add(node.GUID, node);

                if (!(node is DialogueTextNode textNode))
                    continue;
                TextNodeData textNodeData = (TextNodeData)nodeData;
                for (int i = 0; i < textNodeData.Options.Count; i++)
                {
                    OptionData option = textNodeData.Options[i];
                    textNode.CreateOutputOption(option.ResponseText);
                }
            }

            foreach (var connection in saveData.Connections)
            {
                var outputNode = nodeLookup[connection.OutputNodeGUID];
                var inputNode = nodeLookup[connection.InputNodeGUID];

                List<Port> ports = outputNode.mainContainer.Query<Port>().Build().ToList();

                var outputPort = ports[connection.OutputNodeIndex];

                var inputPort = inputNode.inputContainer.Q<Port>();

                var edge = new Edge
                {
                    output = outputPort,
                    input = inputPort
                };
                edge.input.Connect(edge);
                edge.output.Connect(edge);

                graphView.AddElement(edge);
            }
        }

        public static T FindParentOfType<T>(VisualElement element) where T : VisualElement
        {
            var current = element.parent;
            while (current != null)
            {
                if (current is T match)
                    return match;
                current = current.parent;
            }
            return null;
        }
    }
}
