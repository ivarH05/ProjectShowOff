using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(NPC))]
    public class Character : MonoBehaviour
    {
        private NavMeshAgent _agent;
        public NPC npc { get; private set; }

        public float RemainingDistance => _agent.remainingDistance;
        public void SetDestination(Vector3 destination) => _agent.SetDestination(destination);

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }
    }
}
