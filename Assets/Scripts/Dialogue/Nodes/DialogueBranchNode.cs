using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public class BranchNodeData : NodeData
    {
        public BranchNodeData(NodeData data)
        {
            this.type = data.type;
            this.GUID = data.GUID;
            this.Position = data.Position;
        }

        [SerializeReference]
        public DialogueConditionFlag flag;
    }

    [System.Serializable]
    public class BranchData : ScriptableObject
    {
        [SerializeField]
        public DialogueConditionFlag flag;
    }
}
