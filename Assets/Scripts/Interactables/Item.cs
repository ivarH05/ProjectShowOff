using Player;
using UnityEngine;

namespace Interactables
{
    public class Item : Interactable
    {
        public override void OnInteract(PlayerController controller)
        {

        }

        public override void OnUseStart(PlayerController controller) { }
        public override void OnUse(PlayerController controller) { }
        public override void OnUseStop(PlayerController controller) { }
    }
}
