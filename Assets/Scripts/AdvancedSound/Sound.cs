using System;
using UnityEngine;

namespace AdvancedSound
{
    [CreateAssetMenu(menuName = "Sound")]
    public class Sound : ScriptableObject
    {
        public AudioClip Clip;
        public SoundType Type;
        public float Volume = 1;
        public float Pitch = 1;
        public float AudibleRange = 1;
        [Range(0, 1)] public float FaintThreshold;
        [Range(0, 1)] public float ModerateThreshold;
        [Range(0, 1)] public float LoudThreshold;
    }
}