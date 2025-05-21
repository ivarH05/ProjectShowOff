using Player;
using Player.InventoryManagement;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(Rigidbody))] 
    public class ItemObject : Interactable
    {
        public Item item;
        public new Rigidbody rigidbody {  get; private set; }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        public override void OnInteract(PlayerController controller)
        {
            controller.Inventory.PickupItem(this);
        }

        public override void OnUseStart(PlayerController controller) { }
        public override void OnUse(PlayerController controller) { }
        public override void OnUseStop(PlayerController controller) { }
    }
}
