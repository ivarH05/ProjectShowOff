using UnityEngine;
using UnityEngine.Events;

namespace Player.InventoryManagement
{
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    public class Item : ScriptableObject
    {
        public GameObject DefaultWorldObjectPrefab;
        public GameObject HandHeldPrefab;
        public Vector3 baseRightOffset;
        public Vector3 baseLeftOffset;
        public Vector3 baseEulerAngles;

        public UnityEvent OnPickUp = new UnityEvent();
        [Tooltip("called when the item gets released from the inventory, think about dropping, placing in a frame, etc. ")]
        public UnityEvent OnLose = new UnityEvent();
    }
}
