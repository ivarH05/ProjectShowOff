using AdvancedSound;
using Player;
using UnityEngine;

namespace NPC
{
    public class ChasePlayerVisionStrategy : VisionBehaviourStrategy
    {
        public override void OnNoticePlayer(Enemy enemy, PlayerController player)
        {
            Clue clue = new Clue(player, ClueType.PlayerSeen);
            enemy.AddClue(clue);
            enemy.SetBehaviourState<ChasingState>();
        }
    }
}