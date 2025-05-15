using System.Numerics;

namespace NPC
{
    public enum ClueType { MovedItem, PlayerSeen, }
    public class Clue
    {
        public Vector3 position;
        public float time;
        public ClueType type;
    }
}
