using System;
using UnityEngine;

namespace AdvancedSound
{
    [Serializable]
    public struct Sound
    {
        public AudioClip Clip;
        public Vector3 Origin;
        public SoundType Type;
        public float Volume;
        public float Pitch;
    }
}