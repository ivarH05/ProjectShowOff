using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public class OptionData
    {
        public string ResponseText;
        public int index;
    }

    [System.Serializable]
    public class ConnectionData
    {
        public string OutputNodeGUID;
        public int OutputNodeIndex;
        public string InputNodeGUID;
    }
}
