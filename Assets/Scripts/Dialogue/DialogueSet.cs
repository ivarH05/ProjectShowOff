using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "DialogueSet", menuName = "Dialogue/DialogueSet")]
    public class DialogueSet : ScriptableObject
    {
        public ConditionalDialogue[] Dialogues;

        public Dialogue GetDialogue()
        {
            for (int i = 0; i < Dialogues.Length; i++)
                if (Dialogues[i].isActive())
                    return Dialogues[i].dialogue;
            return null;
        }
    }

    [System.Serializable]
    public class ConditionalDialogue
    {
        public ConditionFlagCheck[] conditions;
        public Dialogue dialogue;

        public bool isActive()
        {
            for (int i = 0; i < conditions.Length; i++)
                if (!conditions[i].IsExpected())
                    return false;
            return true;
        }
    }

    [System.Serializable]
    public class ConditionFlagCheck
    {
        public DialogueConditionFlag condition;

        public bool expectedValue;

        public bool IsExpected() { return condition.Get() == expectedValue; }
    }
}