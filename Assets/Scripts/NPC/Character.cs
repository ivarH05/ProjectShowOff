using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace NPC
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class Character : MonoBehaviour
    {
        /**                 SERIALIZED PUBLICS              */
        [Header("Base Variables")]
        public new string name;
        public float baseHealth = 100;
        [Space]
        [SerializeField] public Events events = new Events();


        /**                 PRIVATES              */
        private NavMeshAgent _agent;
        private BehaviourState _behaviourState;


        /**                 UNSERIALIZED PUBLICS              */
        public float health { get; private set; }
        public Character character { get; private set; }
        public float RemainingDistance => _agent.remainingDistance;
        public void SetDestination(Vector3 destination) => _agent.SetDestination(destination);


        /**                 METHODS              */

        internal virtual void Start()
        {
            AddCharacter(this);
            _agent = GetComponent<NavMeshAgent>();

            events.OnSpawn.Invoke(this);
            SetBehaviourState<RoamState>();
        }

        internal virtual void Update() { _behaviourState?.UpdateState(this); }

        internal virtual void OnDestroy()
        {
            events.OnDespawn.Invoke(this);

            RemoveCharacter(this); 
        }

        public void SetBehaviourState<T>() where T : BehaviourState, new()
        {
            _behaviourState?.StopState(this);

            T newState = GetComponent<T>();
            _behaviourState = newState == null ? new T() : newState;

            _behaviourState.StartState(this);
        }

        public virtual void Damage(float damage)
        {
            health -= damage;

            events.OnDamage.Invoke(this, damage);

            if (health < 0)
                Death();
        }

        internal virtual void Death()
        {
            events.OnDeath.Invoke(this);
        }


        /**                 SUBCLASSES              */

        [System.Serializable]
        public class Events
        {
            public UnityEvent<Character> OnSpawn;
            public UnityEvent<Character, float> OnDamage;
            public UnityEvent<Character> OnDeath;
            public UnityEvent<Character> OnDespawn;
        }


        /**                 STATIC              */

        private static List<Character> Characters = new List<Character>();
        public static int CharacterCount => Characters.Count;

        private static void RemoveCharacter(Character character) => Characters.Remove(character);
        private static void AddCharacter(Character character) => Characters.Add(character);
    }
}
