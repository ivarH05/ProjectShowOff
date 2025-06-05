using AdvancedSound;
using UnityEngine;

namespace DialogueSystem
{
    public enum DialogueEventType { Disabled, SetConditionFlag, PlaySound, ShakeCamera, AdvanceTime }

    [System.Serializable]
    public class DialogueEvent
    {
        public DialogueEventType type = DialogueEventType.SetConditionFlag;

        // set condition flag
        public DialogueConditionFlag flag;
        public bool flagValue = true;

        // play sound
        public Sound sound;

        // camera shake
        public float shakeMagnitude;


        public void Invoke()
        {
            switch (type)
            {
                case DialogueEventType.Disabled:
                    break;
                case DialogueEventType.SetConditionFlag:
                    flag.Set(flagValue);
                    break;
            }
        }
    }
}