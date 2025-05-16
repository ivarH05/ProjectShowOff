using Player;
using System.Collections.Generic;
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
    }
}
