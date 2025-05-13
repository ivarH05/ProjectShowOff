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
}
