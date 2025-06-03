using Player;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Crypts;

namespace NPC
{
    public class TrackingState : BehaviourState
    {
        public Vector2 SearchDistanceRange = new Vector2(0, 5);
        public Vector2 SearchTimeRange = new Vector2(15, 35);
        public Vector2 PauseTimeRange = new Vector2(0.5f, 2);
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
            SearchClue(enemy.GetClue(enemy.ClueCount - 1));
        }

        public override void UpdateState(Character character)
        {
            _searchTimer -= Time.deltaTime;
            if (enemy.RemainingDistance > 0.1)
                return;
            _pauseTimer -= Time.deltaTime;
            if (_searchTimer < 0)
                SearchNewClue();
            else if (_pauseTimer < 0)
                ContinueCurrentSearch();
        }

        private void ContinueCurrentSearch()
        {
            SetDestination(_currentClue.GetPositionInBounds(5));

            _pauseTimer = Random.Range(PauseTimeRange.x, PauseTimeRange.y);
        }

        private void SearchNewClue()
        {
            int randomIndex = Random.Range(0, enemy.ClueCount);
            Clue clue = enemy.GetClue(randomIndex);

            if (_searchedCount >= 10 || clue == _currentClue)
            {
                enemy.SetBehaviourState<RoamState>();
                return;
            }
            _searchedCount++;

            SearchClue(clue);
        }

        private void SearchClue(Clue clue)
        {
            _searchTimer = Random.Range(SearchTimeRange.x, SearchTimeRange.y);
            _pauseTimer = Random.Range(PauseTimeRange.x, PauseTimeRange.y);
            _currentClue = clue;

            SetDestination(clue.GetPositionInBounds());
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
