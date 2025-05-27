using OpenCover.Framework.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using static DialogueSystem.Dialogue;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeReference]
        public List<NodeData> Nodes = new List<NodeData>();
        public List<ConnectionData> Connections = new List<ConnectionData>();
        public string a;
    }
}
