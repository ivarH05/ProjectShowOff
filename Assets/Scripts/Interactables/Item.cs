using Player;
using Player.InventoryManagement;
using UnityEngine;

namespace Interactables
{
    public class ItemObject : Interactable
    {
        public Item item;
        public override void OnInteract(PlayerController controller)
        {
            controller.Inventory.PickupItem(this);
            Destroy(gameObject);
        }

        public override void OnUseStart(PlayerController controller) { }
        public override void OnUse(PlayerController controller) { }
        public override void OnUseStop(PlayerController controller) { }
    }
}
