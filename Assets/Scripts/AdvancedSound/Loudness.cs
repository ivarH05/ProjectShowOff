using UnityEngine;

namespace AdvancedSound
{
    /// <summary>
    /// How loud a heard sound was.
    /// </summary>
    public enum Loudness
    {
        /// <summary>
        /// Should not be reacted to.
        /// </summary>
        Inaudible = 0,
        /// <summary>
        /// Can barely be heard.
        /// </summary>
        Faint = 1,
        /// <summary>
        /// Can be easily heard.
        /// </summary>
        Moderate = 2,
        /// <summary>
        /// Is impossible to ignore.
        /// </summary>
        Loud = 3,
    }

    public static class LoudnessFunctions
    {
        public static bool LouderThan(this Loudness loudness, Loudness other) => loudness > other;
        public static bool LouderOrEqual(this Loudness loudness, Loudness other) => loudness >= other;
    }
}
