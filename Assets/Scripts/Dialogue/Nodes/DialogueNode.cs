using UnityEngine;

namespace DialogueSystem
{
    public enum NodeType { None, Text, Branch, Start, End, Event, Reroute }

    [System.Serializable]
    public class NodeData
    {
        public string GUID;
        public NodeType type;
        public Vector2 Position;
    }
}