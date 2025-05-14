using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NPC
{
    public class NPC : MonoBehaviour
    {

        [SerializeField] public Events events = new Events();

        public float health { get; private set; }
        public Character character { get; private set; }

        private BehaviourState _behaviourState;


        /**                 METHODS              */

        private void Awake()
        {
            AddNPC(this);
        }

        private void OnDestroy()
        {
            RemoveNPC(this);
        }

        public void SetBehaviourState<T>() where T : BehaviourState, new()
        {
            _behaviourState?.StopState(this);
            _behaviourState = new T();
            _behaviourState.StartState(this);
        }

        /**                 SUBCLASSES              */

        [System.Serializable]
        public class Events
        {
            public UnityEvent OnSpawn;
            public UnityEvent OnDamage;
            public UnityEvent OnDeath;
            public UnityEvent OnDespawn;
        }


        /**                 STATIC              */

        private static List<NPC> npcs = new List<NPC>();

        public static int NPCCount => npcs.Count;

        private static void RemoveNPC(NPC npc) => npcs.Remove(npc);
        private static void AddNPC(NPC npc) => npcs.Add(npc);
    }
}
