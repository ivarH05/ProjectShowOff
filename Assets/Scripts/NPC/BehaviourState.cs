using UnityEngine;

namespace NPC
{
    public abstract class BehaviourState : MonoBehaviour
    {
        public abstract void StartState(Character character);
        public abstract void UpdateState(Character character);
        public abstract void StopState(Character character);
    }
}
