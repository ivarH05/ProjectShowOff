using System;
using UnityEngine;

namespace AdvancedSound
{
    public class SoundListener : MonoBehaviour
    {
        /// <summary>
        /// Anyone can call this. Try not doing that or im calling you a dog
        /// </summary>
        public Action<Sound> OnHearSound;
    }
}
