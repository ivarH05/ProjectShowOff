using UnityEngine;

namespace DialogueSystem
{
    public enum DialogueEventType { Disabled, SetConditionFlag }

    [System.Serializable]
    public class DialogueEvent
    {
        [SerializeField]
        public DialogueEventType type = DialogueEventType.SetConditionFlag;

        [SerializeField]
        public DialogueConditionFlag flag;

        [SerializeField]
        public bool _boolean = true;

        public void Invoke()
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