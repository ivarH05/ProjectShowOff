using Player;
using Interactables;
using UnityEngine;
using Player.InventoryManagement;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

namespace Player.InventoryManagement
{
    [RequireComponent(typeof(PlayerController))]
    public class Inventory : MonoBehaviour
    {
        public PlayerController player { get; private set; }
        public int inventorySize = 2;
        [SerializeField] private Vector3 LeftHandPosition;
        [SerializeField] private Vector3 RightHandPosition;
        [SerializeField] private Item BeginnersItem;

        private Slot[] _slots;
        private int RightHandItem = 0;
        private int LeftHandItem = 1;

        public Item activeItem => _slots[RightHandItem].item;
        private Slot activeSlot => _slots[RightHandItem];

        private new Camera camera;

        private void Awake()
        {
            camera = Camera.main;

            player = GetComponent<PlayerController>();
            _slots = new Slot[inventorySize];
            for (int i = 0; i < inventorySize; i++)
                _slots[i] = new Slot();

            if (BeginnersItem != null)
                PickupItem(BeginnersItem);
        }

        private void Update()
        {
            LerpItems();
        }

        public void SwitchItem(InputAction.CallbackContext context)
        {
            if (context.ReadValue<Vector2>().y < 0)
                return;
            if (RightHandItem == 0)
                (RightHandItem, LeftHandItem) = (1, 0);
            else
                (RightHandItem, LeftHandItem) = (0, 1);

            SwitchItemPositions();
        }
        public void OnItem0(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;
            if (RightHandItem == 0)
                return;

            RightHandItem = 0;
            LeftHandItem = 1;

            SwitchItemPositions();
        }

        public void OnItem1(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;
            if (RightHandItem == 1)
                return;

            RightHandItem = 1;
            LeftHandItem = 0;

            SwitchItemPositions();
        }

        public void OnDrop(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;
            if (!activeSlot.Occupied)
                return;
            activeSlot.DropItem(player);
        }

        private void SwitchItemPositions()
        {
            if (_slots[LeftHandItem].Occupied)
                _slots[LeftHandItem].handHeldObject.transform.localPosition = GetLeftHandTargetLocation(new Vector3(0, -0.35f, 0));
            if (_slots[RightHandItem].Occupied)
                _slots[RightHandItem].handHeldObject.transform.localPosition = GetRightHandTargetLocation(new Vector3(0, -0.25f, 0));
        }

        private void LerpItems()
        {
            if (_slots[RightHandItem].Occupied)
                LerpItem(_slots[RightHandItem].handHeldObject, GetRightHandTargetLocation(), GetRightHandTargetRotation());

            if (_slots[LeftHandItem].Occupied)
                LerpItem(_slots[LeftHandItem].handHeldObject, GetLeftHandTargetLocation(), GetLeftHandTargetRotation());
        }

        private void LerpItem(GameObject obj, Vector3 pos, Vector3 euler)
        {
            obj.transform.localPosition = Vector3.Lerp(obj.transform.localPosition, pos, Time.deltaTime * 40);
            obj.transform.localRotation = Quaternion.Slerp(obj.transform.localRotation, Quaternion.Euler(euler), Time.deltaTime * 5);
        }

        private Vector3 GetLeftHandTargetLocation(Vector3 additionalOffset = default)
        {
            Vector3 offset = LeftHandPosition + additionalOffset;
            if (_slots[LeftHandItem].Occupied)
                offset += _slots[LeftHandItem].item.baseLeftOffset;

            return offset;
        }

        private Vector3 GetRightHandTargetLocation(Vector3 additionalOffset = default)
        {
            Vector3 offset = RightHandPosition + additionalOffset;
            if (_slots[RightHandItem].Occupied)
                offset += _slots[RightHandItem].item.baseRightOffset;

            return offset;
        }

        private Vector3 GetLeftHandTargetRotation()
        {
            Vector3 offset = Vector3.zero;
            if (_slots[LeftHandItem].Occupied)
                offset = _slots[LeftHandItem].item.baseEulerAngles;

            return offset;
        }

        private Vector3 GetRightHandTargetRotation()
        {
            Vector3 offset = Vector3.zero;
            if (_slots[RightHandItem].Occupied)
                offset = _slots[RightHandItem].item.baseEulerAngles;

            return offset;
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

        public void PickupItem(Item item)
        {
            Slot slot = GetEmptySlot();
            if (slot == null)
            {
                DropItem(activeSlot);
                slot = activeSlot;
            }
            slot.SetItem(item);
        }
        public void UseActiveItem()
        {
            activeSlot.Clear();
        }

        private void DropItem(Slot slot)
        {
            slot.DropItem(player);
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
            public ItemObject worldObject { get; private set; }

            public void Clear()
            {
                item = null;
                if (handHeldObject != null)
                    Destroy(handHeldObject);
            }

            public void SetItem(Item i, ItemObject obj)
            {
                item = i;

                GameObject newObject = Instantiate(item.HandHeldPrefab, Camera.main.transform);
                newObject.transform.position = obj.transform.position;
                newObject.transform.eulerAngles = obj.transform.eulerAngles;

                if (handHeldObject != null)
                    Destroy(handHeldObject);
                handHeldObject = newObject;

                worldObject = obj;
                worldObject.gameObject.SetActive(false);
            }
            public void SetItem(Item i)
            {
                item = i;

                GameObject newObject = Instantiate(item.HandHeldPrefab, Camera.main.transform);
                handHeldObject = newObject;

                if (worldObject != null)
                    Destroy(worldObject.gameObject);

                worldObject = Instantiate(item.DefaultWorldObjectPrefab).GetComponent<ItemObject>();
                worldObject.gameObject.SetActive(false);
            }

            public void DropItem(PlayerController controller)
            {
                if(!Occupied) return;

                worldObject.gameObject.SetActive(true);
                worldObject.transform.position = controller.CameraTransform.position + controller.CameraTransform.forward * 0.25f;
                worldObject.rigidbody.linearVelocity = controller.Body.linearVelocity + controller.CameraTransform.forward * 2.5f;
                Clear();
            }

            public bool Occupied => item != null;
        }
    }
}
