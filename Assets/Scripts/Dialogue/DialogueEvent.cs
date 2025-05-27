using UnityEngine;

namespace DialogueSystem
{
    public enum DialogueEventType { Disabled, SetConditionFlag }

    [System.Serializable]
    public class DialogueEvent
    {
        public DialogueEventType type = DialogueEventType.SetConditionFlag;

        public DialogueConditionFlag flag;
        public bool _boolean = true;

        public void Run()
        {
            switch (type)
            {
                case DialogueEventType.Disabled:
                    break;
                case DialogueEventType.SetConditionFlag:
                    flag.Set(_boolean);
                    break;
            }
        }
    }
}