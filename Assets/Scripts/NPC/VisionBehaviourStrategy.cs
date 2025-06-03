using Player;
using UnityEngine;

namespace NPC
{
    public abstract class VisionBehaviourStrategy : MonoBehaviour
    {
        public abstract void OnNoticePlayer(Enemy enemy, PlayerController player);
    }
}