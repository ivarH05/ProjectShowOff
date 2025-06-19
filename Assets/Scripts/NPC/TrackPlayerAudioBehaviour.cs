using AdvancedSound;
using UnityEngine;

namespace NPC
{
    public class TrackPlayerAudioBehaviour : AudioBehaviourStrategy
    {
        public float hearingErrorMargin = 4;

        public float hearingThreshold = 3;
        public override void OnHearSound(Enemy enemy, HeardSound sound)
        {
            if (sound.Type != SoundType.FootSteps)
                return;

            float loudness = (int)sound.Loudness;
            float distance = Vector3.Distance(sound.Origin, enemy.transform.position);

            float distanceBasedLoudness = Mathf.Sqrt(distance) / loudness;

            if (distanceBasedLoudness < hearingThreshold)
                return;
            Clue c = new Clue()
            {
                position = sound.Origin,
                errorMargin = distanceBasedLoudness * hearingErrorMargin,
                time = Time.time,
                lingerTime = 100 - loudness * loudness * 5,
                type = ClueType.PlayerHeard
            };
            enemy.AddClue(c);
            enemy.SetDestination(c.GetPositionInBounds());
            enemy.SetBehaviourState<ChasingState>();
        }
    }
}
