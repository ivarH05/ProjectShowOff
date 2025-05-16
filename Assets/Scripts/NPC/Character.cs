using GameManagement;
using Player;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        [SerializeField]private BehaviourState _behaviourState;

        [Header("vision")]
        public float visionRange = 15;
        public float fieldOfView = 130;
        public Vector3 eyeOffset;

        [Space]
        [SerializeField] public Events events = new Events();


        /**                 PRIVATES              */
        private NavMeshAgent _agent;
        private HashSet<PlayerController> visiblePlayers = new HashSet<PlayerController>();

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
            _behaviourState?.StartState(this);
        }

        internal virtual void Update() 
        {
            _behaviourState?.UpdateState(this);
            HandlePlayerVisibility();
        }

        private void HandlePlayerVisibility()
        {
            for (int i = 0; i < PlayerManager.PlayerCount; i++)
            {
                PlayerController player = PlayerManager.GetPlayer(i);
                bool seesPlayer = SeesPlayer(player);

                if (seesPlayer)
                {
                    if (visiblePlayers.Contains(player))
                        return;
                    events.OnNoticePlayer.Invoke(this, player);
                    visiblePlayers.Add(player);
                }
                else
                {
                    if (!seesPlayer)
                        visiblePlayers.Remove(player);
                    return;
                }
            }
        }

        Vector2[] visionOffsets = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),

            new Vector2(0.7f, 0.0f),
            new Vector2(-0.7f, 0.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(0.0f, -1.0f),

            new Vector2(0.0f, 2.5f),
            new Vector2(0.0f, -2.5f),

            new Vector2(0.7f, 1.25f),
            new Vector2(0.7f, -1.25f),
            new Vector2(-0.7f, 1.25f),
            new Vector2(-0.7f, -1.25f),
        };

        private bool SeesPlayer(PlayerController player)
        {
            Vector3 eyePos = transform.TransformPoint(eyeOffset);
            Vector3 up = transform.up;
            Vector3 right = transform.right;
            Vector3 forwards = transform.forward;
            Vector3 playerPos = player.transform.position;

            for (int i = 0; i < visionOffsets.Length; i++)
            {
                const float multiplier = 0.25f;
                Vector2 vision = visionOffsets[i] * multiplier;
                Vector3 targetPoint = playerPos + vision.x * right + vision.y * up;

                Vector3 direction = targetPoint - eyePos;
                if (Vector3.Angle(direction, forwards) > fieldOfView / 2)
                    continue;

                RaycastHit hit;
                if (!Physics.Raycast(eyePos, direction, out hit, visionRange))
                {
                    Debug.DrawLine(eyePos, eyePos + direction * visionRange, Color.red);
                    continue;
                }

                if (hit.transform.gameObject == player.gameObject)
                    Debug.DrawLine(eyePos, hit.point, Color.green);
                else
                    Debug.DrawLine(eyePos, hit.point, Color.red);
            }
            return true;
        }

        internal virtual void OnDestroy()
        {
            events.OnDespawn.Invoke(this);

            RemoveCharacter(this); 
        }

        public void SetBehaviourState<T>() where T : BehaviourState, new()
        {
            _behaviourState?.StopState(this);
            T newState = GetComponent<T>();
            _behaviourState = newState == null ? transform.AddComponent<T>() : newState;
            _behaviourState.StartState(this);

            events.OnChangeState.Invoke(this, _behaviourState);
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

        private void OnDrawGizmos()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(eyeOffset), transform.rotation, transform.lossyScale);
            Gizmos.DrawFrustum(Vector3.zero, fieldOfView, visionRange, 0.1f, 1f);
        }

        /**                 SUBCLASSES              */

        [System.Serializable]
        public class Events
        {
            public UnityEvent<Character> OnSpawn;
            public UnityEvent<Character, float> OnDamage;
            public UnityEvent<Character> OnDeath;
            public UnityEvent<Character> OnDespawn;
            [Space]
            public UnityEvent<Character, PlayerController> OnHearPlayer;
            public UnityEvent<Character, PlayerController> OnNoticePlayer;
            public UnityEvent<Character, PlayerController> OnLosePlayer;
            [Space]
            public UnityEvent<Character, BehaviourState> OnChangeState;
        }


        /**                 STATIC              */

        private static List<Character> Characters = new List<Character>();
        public static int CharacterCount => Characters.Count;

        private static void RemoveCharacter(Character character) => Characters.Remove(character);
        private static void AddCharacter(Character character) => Characters.Add(character);
    }
}
