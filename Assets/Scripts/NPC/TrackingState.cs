using Player;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace NPC
{
    public class TrackingState : BehaviourState
    {
        public Vector2 SearchDistanceRange = new Vector2(0, 5);
        public Vector2 SearchTimeRange = new Vector2(5, 15);
        public Vector2 PauseTimeRange = new Vector2(1, 3);
        public bool StayOnTiles = true;

        private float _searchTimer;
        private float _pauseTimer;
        private int _searchedCount;
        private Clue _currentClue;

        Enemy enemy;
        public override void StartState(Character character)
        {
            if (!(character is Enemy e))
            {
                character.SetBehaviourState<RoamState>();
                return;
            }
            enemy = e;

            _searchedCount = 0;
            _searchTimer = Random.Range(SearchTimeRange.x, SearchTimeRange.y);
            _pauseTimer = Random.Range(PauseTimeRange.x, PauseTimeRange.y);
        }

        public override void UpdateState(Character character)
        {
            if (enemy.RemainingDistance > 0.1)
                return;
            if (_searchTimer < 0)
                SearchNewClue(character);

        }

        private void SearchNewClue(Character character)
        {
            int randomIndex = Random.Range(0, enemy.ClueCount);
            Clue clue = enemy.GetClue(randomIndex);

            if (_searchedCount >= 3 || clue == _currentClue)
            {
                character.SetBehaviourState<RoamState>();
                return;
            }
            _searchedCount++;
            _searchTimer = Random.Range(SearchTimeRange.x, SearchTimeRange.y);
            _pauseTimer = Random.Range(PauseTimeRange.x, PauseTimeRange.y);

            SetDestination(clue.position);
        }

        void SetDestination(Vector3 point)
        {
            if(StayOnTiles)
                point = Crypt.GetClosestPoint(point);

            enemy.SetDestination(point);
        }

        public override void StopState(Character character) { }

    }
}
