using Player;
using Player.InventoryManagement;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    [RequireComponent(typeof(Rigidbody))] 
    public class ItemObject : Interactable
    {
        public Item item;
        public bool skipInventory = false;

        public UnityEvent<Item> onGrab = new UnityEvent<Item>();
        public new Rigidbody rigidbody {  get; private set; }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        public override void OnInteract(PlayerController controller)
        {
            if(!skipInventory)
                controller.Inventory.PickupItem(this);
            onGrab.Invoke(item);
        }

        public override void OnUseStart(PlayerController controller) { }
        public override void OnUse(PlayerController controller) { }
        public override void OnUseStop(PlayerController controller) { }
    }
}
