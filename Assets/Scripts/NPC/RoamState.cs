using Crypts;
using UnityEngine;

namespace NPC
{
    public class RoamState : BehaviourState
    {
        public Vector2 DistanceRange = new Vector2(5, 15);
        public Vector2 PauseTimeRange = new Vector2(2, 5);

        private float _timer;

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
            Vector3 randomPosition = GetRandomPosition(DistanceRange.x, DistanceRange.y);
            Vector3 nextPos = character.transform.position + randomPosition;

            Vector3 tiledPos = Crypt.GetClosestPoint(nextPos);

            character.SetDestination(tiledPos);
        }
    }
}