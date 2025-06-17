using Player;
using Player.InventoryManagement;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public abstract class Placable : Interactable
    {
        internal Item _placedItem;

        [SerializeField] private bool canRemove = true;
        [SerializeField] private bool canPlace = true;
        [Space]
        [SerializeField] internal bool isPlaced = false;
        [Space]
        public Events events;
        public virtual bool CanPlace(Item i) => canPlace;
        public virtual bool CanRemove(Item i) => canRemove;

        public override void OnInteract(PlayerController controller)
        {
            if (isPlaced)
            {
                if (!CanRemove(_placedItem))
                    return;

                Remove(_placedItem);
                controller.Inventory.PickupItem(_placedItem);
                events.OnRemove.Invoke(this, _placedItem, controller);
                _placedItem = null;
                isPlaced = false;
                return;
            }
            else
            {
                Item item = controller.Inventory.activeItem;
                if (item == null)
                    return;
                if (!CanPlace(item))
                    return;
                controller.Inventory.UseActiveItem();
                events.OnPlace.Invoke(this, item, controller);
                SetItem(item);
            }
        }

        protected void SetItem(Item item)
        {
            Place(item);
            _placedItem = item;
            isPlaced = true;
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
