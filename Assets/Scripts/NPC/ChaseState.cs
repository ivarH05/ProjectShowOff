using Player;
using UnityEngine;

namespace NPC
{
    public class ChasingState : BehaviourState
    {
        private float _lastSeenTime = 0;
        private PlayerController _trackingPlayer;
        public override void StartState(Character character)
        {
            base.StartState(character);
            _trackingPlayer = null;
            character.events.WhileSeePlayer.AddListener(WhileSeePlayer);
        }
        public override void UpdateState(Character character)
        {
            if (_lastSeenTime > 1 && character.RemainingDistance < 0.1f)
                StopChasing(character);
            _lastSeenTime += Time.deltaTime;
        }
        public override void StopState(Character character) 
        {
            character.events.WhileSeePlayer.RemoveListener(WhileSeePlayer);
        }

        void StopChasing(Character character)
        {
            if (!(character is Enemy e))
                return;
            if(_trackingPlayer != null)
                e.AddClue(new Clue(_trackingPlayer, ClueType.PlayerSeen));
            character.SetBehaviourState<TrackingState>();
        }

        void WhileSeePlayer(Character character, PlayerController player)
        {
            if (_trackingPlayer == null)
                _trackingPlayer = player;
            character.SetDestination(player.transform.position);
            _lastSeenTime = 0;
        }
    }
}
