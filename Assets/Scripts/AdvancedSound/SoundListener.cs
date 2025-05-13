using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace AdvancedSound
{
    public class SoundListener : MonoBehaviour
    {
        /// <summary>
        /// All active listeners in the scene.
        /// </summary>
        public static ReadOnlyCollection<SoundListener> Listeners => _listeners.AsReadOnly();
        static List<SoundListener> _listeners = new();

        /// <summary>
        /// Anyone can call this. Try not doing that or im calling you a dog
        /// </summary>
        public Action<Sound, Loudness> OnHearSound;

        private void OnEnable() => _listeners.Add(this);
        private void OnDisable() => _listeners.Remove(this);

    }
}
