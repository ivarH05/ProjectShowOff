using UnityEngine;

namespace DialogueSystem
{
    public class DialoguePlayer : MonoBehaviour
    {
        private Dialogue _activeDialogue;

        public NodeData activeNode;

        public void SetDialogue(Dialogue dialogue)
        {
            Debug.Log("Set");
            _activeDialogue = dialogue;
        }

        public void StartDialogue()
        {
            Debug.Log("Started");
            activeNode = _activeDialogue.GetStartNode();
        }

        public void Next(int index)
        {
            Debug.Log("Updated");
            activeNode = _activeDialogue.ContinueNode(activeNode, index);
        }
    }
}
