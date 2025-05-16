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

            Clue clue = new Clue(player.transform.position, ClueType.PlayerHeard);
            AddClue(clue);
        }

        void OnNoticePlayer(Character character, PlayerController player)
        {
            if (!defaultBehaviour)
                return;

            Clue clue = new Clue(player.transform.position, ClueType.PlayerSeen);
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
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.orange;
            for (int i = 0; i < clues.Count; i++)
                Gizmos.DrawSphere(clues[i].position, 0.25f);
        }
    }
}
