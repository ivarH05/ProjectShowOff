using System;
using UnityEngine;

namespace AdvancedSound
{
    [Serializable]
    public struct Sound
    {
        public AudioClip Clip;
        public SoundType Type;
        public float Volume;
        public float Pitch;
        public float AudibleRange;
        [Range(0, 1)] public float FaintThreshold;
        [Range(0, 1)] public float ModerateThreshold;
        [Range(0, 1)] public float LoudThreshold;
    }
}