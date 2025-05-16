
using UnityEngine;

namespace NPC
{
    public enum ClueType { MovedItem, PlayerSeen, PlayerHeard }
    public class Clue
    {
        public Clue(Vector3 position, ClueType type)
        {
            this.position = position;
            this.type = type;
            time = Time.time;
        }
        public Vector3 position;
        public float time;
        public ClueType type;
    }
}
