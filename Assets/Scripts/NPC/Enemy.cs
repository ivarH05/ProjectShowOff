using Player;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace NPC
{
    public class Enemy : Character
    {
        private List<Clue> clues = new List<Clue>();
        public bool defaultBehaviour = true;

        internal override void Start()
        {
            base.Start();
            events.OnHearPlayer.AddListener(OnHearPlayer);
            events.OnNoticePlayer.AddListener(OnNoticePlayer);
        }
        internal override void OnDestroy()
        {
            base.OnDestroy();
            events.OnHearPlayer.RemoveListener(OnHearPlayer);
            events.OnNoticePlayer.RemoveListener(OnNoticePlayer);
        }

        public Clue GetClue(int i) => clues[i];
        public int ClueCount => clues.Count;

        void OnHearPlayer(Character character, PlayerController player)
        {
            if (!defaultBehaviour)
                return;

            Clue clue = new Clue(player, ClueType.PlayerHeard);
            AddClue(clue);
        }

        void OnNoticePlayer(Character character, PlayerController player)
        {
            if (!defaultBehaviour)
                return;

            Clue clue = new Clue(player, ClueType.PlayerSeen);
            AddClue(clue);
            character.SetBehaviourState<ChasingState>();
        }
        public void AddClue(Clue clue) { clues.Add(clue); }

        internal override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.orange;
            for (int i = 0; i < clues.Count; i++)
                Gizmos.DrawWireSphere(clues[i].position, 0.15f);
        }
        internal override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.orange;
            for (int i = 0; i < clues.Count; i++)
                DrawClue(clues[i]);
        }

        void DrawClue(Clue clue)
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawSphere(clue.position, 0.25f);
            if (clue.type == ClueType.PlayerHeard || clue.direction == Vector3.zero)
                return;

            const float radius = 0.8f;

            List<Vector3> points = new List<Vector3>() { Vector3.zero };
            for (float a = -45; a <= 45; a += 10)
            {
                float rads = a * Mathf.Deg2Rad;
                float x = Mathf.Sin(rads) * radius;
                float z = Mathf.Cos(rads) * radius;

                points.Add(new Vector3(x, 0f, z));
            }

            Quaternion rotation = Quaternion.LookRotation(clue.direction.normalized);
            Gizmos.matrix = Matrix4x4.TRS(clue.position, rotation, Vector3.one);
            Gizmos.DrawLineStrip(points.ToArray(), true);

        }
    }
}
