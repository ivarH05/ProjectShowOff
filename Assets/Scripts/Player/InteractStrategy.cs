
using Interactables;
using UnityEngine;

namespace Player 
{
    public abstract class InteractStrategy : MonoBehaviour
    {
        [HideInInspector]public Interactable activeInteractable;
        public abstract void StartStrategy(PlayerController controller);

        public abstract void OnAttackStart(PlayerController controller);
        public abstract void OnAttack(PlayerController controller);
        public abstract void OnAttackStop(PlayerController controller);

        public abstract void OnAttackSecondary(PlayerController controller);
        public abstract void OnInteract(PlayerController controller);

        public abstract void StopStrategy(PlayerController controller);
    }
}
