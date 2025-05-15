using UnityEngine;

namespace NPC
{
    public class RoamState : BehaviourState
    {
        public Vector2 DistanceRange = new Vector2(5, 15);
        public Vector2 PauseTimeRange = new Vector2(2, 5);

        private float _timer;
        private Vector3 _DebugTargetPosition;

        public override void StartState(Character character) { }
        public override void UpdateState(Character character)
        {
            if(character.RemainingDistance < 0.1f)
            _timer -= Time.deltaTime;
            if (_timer > 0)
                return;

            RoamToNewSpot(character);
            _timer += Random.Range(PauseTimeRange.x, PauseTimeRange.y);
        }
        public override void StopState(Character character) { }

        void RoamToNewSpot(Character character)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            Vector3 relativePosition = new Vector3(direction.x, 0, direction.y) * Random.Range(DistanceRange.x, DistanceRange.y);
            Vector3 nextPos = character.transform.position + relativePosition;
            _DebugTargetPosition = nextPos;

            Vector3 tiledPos = Crypt.GetClosestPoint(nextPos);

            character.SetDestination(tiledPos);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, DistanceRange.x);
            Gizmos.DrawWireSphere(transform.position, DistanceRange.y);

            Gizmos.DrawSphere(_DebugTargetPosition, 0.1f);
        }
    }
}