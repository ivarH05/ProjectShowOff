using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    [System.Serializable]
    public class EventNodeData : NodeData
    {
        public EventNodeData(NodeData data)
        {
            this.type = data.type;
            this.GUID = data.GUID;
            this.Position = data.Position;
        }

        [SerializeField]
        public DialogueEvent onRun = new DialogueEvent();
    }

    [System.Serializable]
    public class EventData : ScriptableObject
    {
        [SerializeField]
        public DialogueEvent onRun = new DialogueEvent();
    }
}
