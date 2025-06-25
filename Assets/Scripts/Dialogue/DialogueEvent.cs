using AdvancedSound;
using Daytime;
using System;
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


        public void Invoke(DialoguePlayer player)
        {
            try
            {
                switch (type)
                {
                    case DialogueEventType.Disabled:
                        break;
                    case DialogueEventType.SetConditionFlag:
                        flag.Set(flagValue);
                        break;
                    case DialogueEventType.PlaySound:
                        player.PlayOneShot(sound.GetClip(), sound.Volume, sound.Pitch);
                        break;
                    case DialogueEventType.ShakeCamera:
                        CameraShake.ShakeCamera(1.15f, shakeMagnitude);
                        break;
                    case DialogueEventType.AdvanceTime:
                        TimeHandler.Advance();
                        break;
                }
            }
            catch(Exception e) { Debug.LogError(e.Message); }
        }
    }
}