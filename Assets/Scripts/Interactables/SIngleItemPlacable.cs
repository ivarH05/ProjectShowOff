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

        [SerializeField] private GameObject placedStateObject;

        public override bool CanPlace(Item item) => (_targetItem == null || _targetItem == item) ? base.CanPlace(item) : false;

        private void Awake()
        {
            if (isPlaced)
                SetItem(_targetItem);
            else
                Remove(_targetItem);
        }

        override internal void Place(Item item)
        {
            placedStateObject.SetActive(true);
        }

        override internal void Remove(Item item)
        {
            placedStateObject.SetActive(false);
        }
    }
}
