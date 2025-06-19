using AdvancedSound;
using UnityEngine;

namespace NPC
{
    public abstract class AudioBehaviourStrategy : MonoBehaviour
    {
        public abstract void OnHearSound(Enemy enemy, HeardSound sound);
    }
}