using Player;
using Interactables;
using UnityEngine;
using Player.InventoryManagement;
using UnityEngine.Events;

namespace Player.InventoryManagement
{
    [RequireComponent(typeof(PlayerController))]
    public class Inventory : MonoBehaviour
    {
        private Slot[] _slots;
        private int _activeItem;

        public PlayerController player { get; private set; }
        public int inventorySize = 2;
        [SerializeField] private Vector3 LeftHandPosition;
        [SerializeField] private Vector3 RightHandPosition;

        public Item activeItem => _slots[_activeItem].item;
        private Slot activeSlot => _slots[_activeItem];

        private void Awake()
        {
            player = GetComponent<PlayerController>();
            _slots = new Slot[inventorySize];
            for (int i = 0; i < inventorySize; i++)
                _slots[i] = new Slot();
        }

        private void Update()
        {
            LerpItems();
        }

        private void LerpItems()
        {
            if (_slots[0].Occupied)
                LerpItem(_slots[0].handHeldObject, GetLeftHandTargetLocation(), GetLeftHandTargetRotation());

            if (_slots[1].Occupied)
                LerpItem(_slots[1].handHeldObject, GetRightHandTargetLocation(), GetRightHandTargetRotation());
        }

        private void LerpItem(GameObject obj, Vector3 pos, Vector3 euler)
        {
            obj.transform.position = Vector3.Lerp(obj.transform.position, pos, Time.deltaTime * 20);
            obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, Quaternion.Euler(euler), Time.deltaTime * 15);
        }

        private void OnDrawGizmos()
        {
            if (player == null)
                player = GetComponent<PlayerController>();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.CameraTransform.TransformPoint(LeftHandPosition), 0.1f);
            Gizmos.DrawWireSphere(player.CameraTransform.TransformPoint(RightHandPosition), 0.1f);
        }

        private Vector3 GetLeftHandTargetLocation()
        {
            Vector3 offset = LeftHandPosition;
            if (_slots[0].Occupied)
                offset += _slots[0].item.baseOffset;

            return player.CameraTransform.TransformPoint(offset);
        }

        private Vector3 GetRightHandTargetLocation()
        {
            Vector3 offset = RightHandPosition;
            if (_slots[1].Occupied)
                offset += _slots[1].item.baseOffset;

            return player.CameraTransform.TransformPoint(offset);
        }

        private Vector3 GetLeftHandTargetRotation()
        {
            Vector3 offset = Vector3.zero;
            if (_slots[0].Occupied)
                offset = _slots[0].item.baseEulerAngles;

            return player.CameraTransform.eulerAngles + offset;
        }

        private Vector3 GetRightHandTargetRotation()
        {
            Vector3 offset = Vector3.zero;
            if (_slots[1].Occupied)
                offset = _slots[1].item.baseEulerAngles;

            return player.CameraTransform.eulerAngles + offset;
        }

        public struct Events
        {
            public static UnityEvent<PlayerController, Item> OnPickupItem = new UnityEvent<PlayerController, Item>();
            public static UnityEvent<PlayerController, Item> OnSwitchItem = new UnityEvent<PlayerController, Item>();
            public static UnityEvent<PlayerController, Item> OnUseItem = new UnityEvent<PlayerController, Item>();
            public static UnityEvent<PlayerController, Item> OnDropItem = new UnityEvent<PlayerController, Item>();
        }

        public void PickupItem(ItemObject itemObject)
        {
            Slot slot = GetEmptySlot();
            if (slot == null)
            {
                DropItem(activeSlot);
                slot = activeSlot;
            }
            slot.SetItem(itemObject.item, itemObject);
        }

        private void DropItem(Slot slot)
        {
            slot.Clear();
        }

        private Slot GetEmptySlot()
        {
            for (int i = 0; i < _slots.Length; i++)
                if (!_slots[i].Occupied)
                    return _slots[i];
            return null;
        }

        private class Slot
        {
            public Item item { get; private set; }

            public GameObject handHeldObject { get; private set; }
            public GameObject worldObject { get; private set; }

            public void Clear()
            {
                item = null;
                if(handHeldObject != null)
                    Destroy(handHeldObject);
            }

            public void SetItem(Item i, ItemObject obj)
            {
                item = i;

                GameObject newObject = Instantiate(item.Prefab);
                newObject.transform.position = obj.transform.position;
                newObject.transform.eulerAngles = obj.transform.eulerAngles;

                handHeldObject = newObject;
            }

            public bool Occupied => item != null;
        }
    }
}
