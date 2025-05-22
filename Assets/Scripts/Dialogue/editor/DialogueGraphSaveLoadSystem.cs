using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.StandaloneInputModule;

namespace DialogueSystem
{
    public static class SaveLoadSystem
    {
        public static void Save(DialogueGraphView graphView)
        {
            string path = EditorUtility.SaveFilePanel("Save Dialogue Graph", "Assets", "DialogueGraph", "json");
            if (path == null || path == "")
                return;

            var saveData = new GraphSaveData();

            // Save nodes
            foreach (var node in graphView.nodes.OfType<DialogueNode>())
            {
                List<OptionData> newOptions = new List<OptionData>();
                var oldOptions = node.options;
                for (int i = 0; i < oldOptions.Count; i++)
                {
                    var option = oldOptions[i];
                    newOptions.Add(new OptionData()
                    {
                        index = i,
                        ResponseText = option.value,
                    });

                    var connections = option.port.connections.ToArray();
                    for (int j = 0; j < connections.Length; j++)
                    {
                        var connection = connections[j];
                        var inputNode = FindParentOfType<DialogueNode>(connection.input);

                        saveData.Connections.Add(new ConnectionData
                        {
                            OutputNodeGUID = node.GUID,
                            OutputNodeIndex = i,
                            InputNodeGUID = inputNode.GUID

                        });
                    }
                }

                saveData.Nodes.Add(new NodeData
                {
                    GUID = node.GUID,
                    DialogueText = node.dialogueText,
                    Position = node.GetPosition().position

                });
            }

            // Save to JSON
            var json = JsonUtility.ToJson(saveData, true); 
            File.WriteAllText(path, json);
        }


        public static void Load(DialogueGraphView graphView)
        {
            string path = EditorUtility.OpenFilePanel("Load Dialogue Graph", "Assets", "json");

            if (path == null)
                return;

            if (!File.Exists(path)) 
                return;

            // Clear existing graph
            graphView.ClearGraph();

            var json = File.ReadAllText(path);
            var saveData = JsonUtility.FromJson<GraphSaveData>(json);

            // First recreate all nodes
            Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();
            foreach (var nodeData in saveData.Nodes)
            {
                DialogueNode node = graphView.CreateNode(nodeData.DialogueText, nodeData.Position);
                node.GUID = nodeData.GUID;
                node.SetText(nodeData.DialogueText);
                nodeLookup[node.GUID] = node;

                for (int i = 0; i < nodeData.Options.Count; i++)
                {
                    OptionData option = nodeData.Options[i];
                    node.CreateOutputOption(option.ResponseText);
                }
            }

            // Then recreate all edges
            foreach (var connection in saveData.Connections)
            {
                var outputNode = nodeLookup[connection.OutputNodeGUID];
                var inputNode = nodeLookup[connection.InputNodeGUID];

                var outputPort = outputNode.mainContainer.Query<Port>().Build().ToArray()[connection.OutputNodeIndex+1];
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
