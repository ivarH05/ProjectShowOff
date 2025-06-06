using UnityEngine;

namespace NPC
{
    public abstract class BehaviourState : MonoBehaviour
    {
        public float speed = 4;
        public virtual void StartState(Character character) { character.agent.speed = speed; }
        public abstract void UpdateState(Character character);
        public abstract void StopState(Character character);

        internal static Vector3 GetRandomPosition(float minDistance, float maxDistance)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            Vector3 relativePosition = new Vector3(direction.x, 0, direction.y) * Random.Range(minDistance, maxDistance);
            return relativePosition;
        }
    }
}
