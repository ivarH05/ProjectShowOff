using Player;
using UnityEngine;

namespace NPC
{
    public class ChasingState : BehaviourState
    {
        float lastSeenPlayer = 0;
        public override void StartState(Character character)
        {
            character.events.WhileSeePlayer.AddListener(WhileSeePlayer);
        }
        public override void UpdateState(Character character)
        {
            if (lastSeenPlayer > 1 && character.RemainingDistance < 0.1f)
                character.SetBehaviourState<TrackingState>();
            lastSeenPlayer += Time.deltaTime;
        }
        public override void StopState(Character character) 
        {
            character.events.WhileSeePlayer.RemoveListener(WhileSeePlayer);
        }
        void WhileSeePlayer(Character character, PlayerController player)
        {
            character.SetDestination(player.transform.position);
            lastSeenPlayer = 0;
        }
    }
}
