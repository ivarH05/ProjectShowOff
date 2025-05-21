using Player;
using Player.InventoryManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class Placable : Interactable
    {
        [SerializeField]private Item _targetItem;

        [SerializeField] private GameObject placedStateObject;
        [SerializeField] private bool placed = false;
        [SerializeField] private bool CanRemove = false;
        [SerializeField] private Events events;


        private void Start()
        {
            if (placed)
                Place();
            else
                Remove();
        }

        public bool CanPlace(Item i) => _targetItem == i;

        public override void OnInteract(PlayerController controller)
        {
            if (placed)
            {
                if (!CanRemove)
                    return;

                Remove();
                controller.Inventory.PickupItem(_targetItem);
                events.OnRemove.Invoke(this, controller);
                return;
            }
            Item item = controller.Inventory.activeItem;
            if (!CanPlace(item))
                return;

            Place();
            controller.Inventory.UseActiveItem();
            events.OnPlace.Invoke(this, controller);
        }

        void Place()
        {
            placedStateObject.SetActive(true);
            placed = true;
        }

        void Remove()
        {
            placedStateObject.SetActive(false);
            placed = false;
        }

        public override void OnUseStart(PlayerController controller) { }
        public override void OnUse(PlayerController controller) { }
        public override void OnUseStop(PlayerController controller) { }

        [System.Serializable]
        public struct Events
        {
            public UnityEvent<Placable, PlayerController> OnPlace;
            public UnityEvent<Placable, PlayerController> OnRemove;
        }
    }
}
