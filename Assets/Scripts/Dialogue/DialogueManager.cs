using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueManager
    {
        private static HashSet<DialogueConditionFlag> _flags;

        public static bool IsTrue(DialogueConditionFlag flag) => _flags.Contains(flag);
        public static bool IsFalse(DialogueConditionFlag flag) => !_flags.Contains(flag);

        public static void SetFlag(DialogueConditionFlag flag, bool value)
        {
            if (_flags.Contains(flag) == value)
                return;

            if(value)
                _flags.Add(flag);
            else
                _flags.Remove(flag);
        }
    }
}
