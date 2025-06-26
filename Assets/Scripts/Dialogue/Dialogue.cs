using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        public Character character;
        [TextArea(5, 15)]
        public string Description;

        [SerializeReference]
        private List<NodeData> _nodes = new List<NodeData>();
        [SerializeField]
        public List<ConnectionData> _connections = new List<ConnectionData>();

        private Dictionary<string, NodeData> _GUIDLookup;


        public List<NodeData> Nodes
        {
            get => _nodes;
            set
            {
                _nodes = value;
            }
        }
        public List<ConnectionData> Connections
        {
            get => _connections;
            set
            {
                _connections = value;
            }
        }

        public NodeData GetStartNode()
        {
            for (int i = 0; i < Nodes.Count; i++)
                if (Nodes[i].type == NodeType.Start)
                    return Nodes[i];
            return null;
        }

        public NodeData ContinueNode(NodeData currentNode, int Selection = 0)
        {
            return GetOutputNodes(currentNode)[Selection];
        }

        private NodeData[] GetOutputNodes(NodeData currentNode)
        {
            var connections = GetOutputConnections(currentNode.GUID);
            var result = new NodeData[connections.Length];

            for (int i = 0; i < connections.Length; i++)
            {
                ConnectionData connection = connections[i];
                int index = connection.OutputNodeIndex;
                NodeData node = GetNodeByGUID(connection.InputNodeGUID);

                result[index] = node;
            }

            return result;
        }
        
        private ConnectionData[] GetOutputConnections(string GUID)
        {
            List<ConnectionData> result = new List<ConnectionData>();

            foreach (ConnectionData connection in _connections)
            {
                if(connection.OutputNodeGUID == GUID) 
                    result.Add(connection);
            }
            return result.ToArray();
        }

        private NodeData GetNodeByGUID(string GUID)
        {
            if(_GUIDLookup == null)
                RecalculateDictionary();
            
            return _GUIDLookup[GUID];
        }

        private void RecalculateDictionary()
        {
            _GUIDLookup = new Dictionary<string, NodeData>();
            foreach (NodeData node in Nodes)
            {
                _GUIDLookup.Add(node.GUID, node);
            }
        }
    }
}
