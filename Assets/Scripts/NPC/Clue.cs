
using Player;
using UnityEngine;

namespace NPC
{
    public enum ClueType { MovedItem, PlayerSeen, PlayerHeard }
    public class Clue
    {
        public Clue(PlayerController player, ClueType type)
        {
            position = player.transform.position;
            if(type == ClueType.PlayerSeen)
                direction = player.Body.linearVelocity.normalized;
            time = Time.time;
            this.type = type;
        }

        public Vector3 position = Vector3.zero;
        public Vector3 direction = Vector3.zero;
        public float time;
        public ClueType type;
    }
}
