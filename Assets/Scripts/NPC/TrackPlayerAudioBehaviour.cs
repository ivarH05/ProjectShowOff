using AdvancedSound;
using UnityEngine;

namespace NPC
{
    public class TrackPlayerAudioBehaviour : AudioBehaviourStrategy
    {
        public float hearingErrorMargin = 4;
        public override void OnHearSound(Enemy enemy, HeardSound sound)
        {
            if (sound.Type != SoundType.FootSteps)
                return;

            int loudness = (int)sound.Loudness;
            float errorMargin = hearingErrorMargin / (loudness * loudness);
            Clue c = new Clue()
            {
                position = sound.Origin,
                errorMargin = errorMargin,
                time = Time.time,
                type = ClueType.PlayerHeard
            };
            enemy.AddClue(c);
            enemy.SetDestination(c.GetPositionInBounds());
            enemy.SetBehaviourState<ChasingState>();
        }
    }
}
