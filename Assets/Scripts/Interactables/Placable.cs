using Player;
using Player.InventoryManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public abstract class Placable : Interactable
    {
        private Item _placedItem;

        [SerializeField] private bool canRemove = true;
        [SerializeField] private bool canPlace = true;
        [Space]
        private bool isPlaced = false;
        [Space]
        public Events events;

        public virtual void CanPlace(Placable self, Item i)
        {
            return self.canPlace;
        }

        public virtual bool CanPlace(Item i) => canPlace;

        public override void OnInteract(PlayerController controller)
        {
            if (isPlaced)
            {
                if (!canRemove)
                    return;

                Remove(_placedItem);
                controller.Inventory.PickupItem(_placedItem);
                events.OnRemove.Invoke(this, _placedItem, controller);
                _placedItem = null;
                return;
            }
            else
            {
                Item item = controller.Inventory.activeItem;
                if (!CanPlace(item))
                    return;

                Place(item);
                _placedItem = item;
                controller.Inventory.UseActiveItem();
                events.OnPlace.Invoke(this, item, controller);
            }
        }

        internal abstract void Place(Item item);

        internal abstract void Remove(Item item);

        public override void OnUseStart(PlayerController controller) { }
        public override void OnUse(PlayerController controller) { }
        public override void OnUseStop(PlayerController controller) { }

        [System.Serializable]
        public struct Events
        {
            public UnityEvent<Placable, Item, PlayerController> OnPlace;
            public UnityEvent<Placable, Item, PlayerController> OnRemove;
        }
    }
}
