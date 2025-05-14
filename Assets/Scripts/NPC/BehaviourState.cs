using UnityEngine;

namespace NPC
{
    public abstract class BehaviourState
    {
        public abstract void StartState(NPC npc);
        public abstract void UpdateState(NPC npc);
        public abstract void StopState(NPC npc);
    }
}
