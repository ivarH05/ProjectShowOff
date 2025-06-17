using Player;
using Player.InventoryManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class SingleItemPlacable : Placable
    {
        [SerializeField] private Item _targetItem;
        private Item _placedItem;
        [SerializeField] private GameObject placedStateObject;

        private void Start()
        {
            if (placed)
                Place();
            else
                Remove();
        }

        public override bool CanPlace(Item item) => (_targetItem == null || _targetItem == item) ? base.CanPlace(item) : false;


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
