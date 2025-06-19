using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "OnSampleConditionMet", menuName = "Dialogue/ConditionFlag")]
    public class DialogueConditionFlag : ScriptableObject
    {
        [TextArea(5, 20)]
        public string description;

        public void Set(bool value)
        {
            DialogueManager.SetFlag(this, value);
        }

        public bool Get()
        {
            return DialogueManager.IsTrue(this);
        }
    }
}
