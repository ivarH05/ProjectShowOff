using Player;
using UnityEngine;

namespace NPC
{
    public enum ClueType { MovedItem, PlayerSeen, PlayerHeard }

    [System.Serializable]
    public class Clue
    {
        public Clue() {}

        public Clue(PlayerController player, ClueType type)
        {
            Update(player, type);
        }

        public Color color = Color.orange;

        public Vector3 position = Vector3.zero;
        public Vector3 direction = Vector3.zero;
        public float errorMargin;
        public float time;
        public float lingerTime = 120;
        public ClueType type;

        public void Update(PlayerController player, ClueType type)
        {
            position = player.transform.position;
            if (type == ClueType.PlayerSeen)
                direction = player.Body.linearVelocity.normalized;
            time = Time.time;
            lingerTime = 250;
            this.type = type;
        }

        public Vector3 GetPositionInBounds()
        {
            Vector2 errorOffset = Random.insideUnitCircle * errorMargin;
            Vector3 randomPosition = new Vector3(position.x + errorOffset.x, position.y, position.z + errorOffset.y);

            return randomPosition;
        }
        public Vector3 GetPositionInBounds(float directionDistance)
        {
            Vector2 errorOffset = Random.insideUnitCircle * errorMargin;
            Vector2 directionOffset = Random.insideUnitCircle * 0.5f;

            Vector3 randomDirection = new Vector3(direction.x + directionOffset.x, direction.y, direction.z + directionOffset.y).normalized;
            Vector3 randomPosition = new Vector3(position.x + errorOffset.x, position.y, position.z + errorOffset.y);

            return randomPosition + randomDirection * directionDistance;
        }
    }
}
