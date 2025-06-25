using DialogueSystem;
using UnityEngine;

namespace Interactables
{
    public class ConditionalSpawner : MonoBehaviour
    {
        public ConditionFlagCheck[] conditions;

        private void Awake()
        {
            gameObject.SetActive(isActive());
        }

        public bool isActive()
        {
            for (int i = 0; i < conditions.Length; i++)
                if (!conditions[i].IsExpected())
                    return false;
            return true;
        }
    }

}