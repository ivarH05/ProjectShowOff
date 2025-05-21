using System;
using UnityEngine;

namespace AdvancedSound
{
    [CreateAssetMenu(menuName = "Sound")]
    public class Sound : ScriptableObject
    {
        public AudioClip[] ClipSelection;
        [SerializeField] RandomSoundSelection SelectionMode;
        public SoundType Type;
        public float Volume = 1;
        public float Pitch = 1;
        public float AudibleRange = 1;
        [Range(0, 1)] public float FaintThreshold;
        [Range(0, 1)] public float ModerateThreshold;
        [Range(0, 1)] public float LoudThreshold;
        int _lastClipIndex;

        public AudioClip GetClip()
        {
            if(ClipSelection == null || ClipSelection.Length < 1)
            {
                Debug.LogError("Couldnt play audio: there were no clips.", this);
                return null;
            }
            switch(SelectionMode)
            {
                case RandomSoundSelection.RandomAvoidRepeats:
                    int attempt = 0;
                    for (int i = 0; i<10; i++)
                    {
                        attempt = UnityEngine.Random.Range(0, ClipSelection.Length);
                        if (attempt == _lastClipIndex) continue;
                        else break;
                    }
                    _lastClipIndex = attempt;
                    return ClipSelection[attempt];
                case RandomSoundSelection.Sequential:
                    _lastClipIndex++;
                    _lastClipIndex %= ClipSelection.Length;
                    return ClipSelection[_lastClipIndex];
                case RandomSoundSelection.FullyRandom:
                default:
                    return ClipSelection[UnityEngine.Random.Range(0,ClipSelection.Length)];
            }
        }

        [Serializable] enum RandomSoundSelection
        {
            FullyRandom, RandomAvoidRepeats, Sequential
        }
    }
}