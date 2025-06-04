using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public class TextNodeData : NodeData
    {
        public TextNodeData(NodeData data)
        {
            this.type = data.type;
            this.GUID = data.GUID;
            this.Position = data.Position;
        }
        public Character speaker;
        public string dialogueText;
        public List<OptionData> Options;
    }

    [System.Serializable]
    public class CharacterData : ScriptableObject
    {
        [SerializeField]
        public Character speaker;
    }
}
