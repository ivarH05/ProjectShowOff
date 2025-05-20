using Player;
using UnityEngine;

namespace Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        public abstract void OnInteract(PlayerController controller);

        public abstract void OnUseStart(PlayerController controller);
        public abstract void OnUse(PlayerController controller);
        public abstract void OnUseStop(PlayerController controller);
    }
}
