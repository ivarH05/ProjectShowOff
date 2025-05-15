using UnityEngine;

namespace NPC
{
    public abstract class BehaviourState : MonoBehaviour
    {
        public abstract void StartState(Character npc);
        public abstract void UpdateState(Character npc);
        public abstract void StopState(Character npc);
    }
}
