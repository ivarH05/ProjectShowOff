using UnityEngine;
using UnityEngine.Events;

namespace Player.InventoryManagement
{
    [CreateAssetMenu(fileName = "Item", menuName = "Interactables/Item")]
    public class Item : ScriptableObject
    {
        [Header("Setup")]
        public GameObject DefaultWorldObjectPrefab;
        public GameObject HandHeldPrefab;
        public Vector3 baseRightOffset;
        public Vector3 baseLeftOffset;
        public Vector3 baseEulerAngles;

        public Vector3 ItemFrameOffset;
        public Vector3 ItemFrameEulerAngles;

        [Header("Settings")]
        [Tooltip("The weight of the item when placed on the scale")]
        public float weight = 0;

        [Header("Events")]

        public UnityEvent OnPickUp = new UnityEvent();
        [Tooltip("called when the item gets released from the inventory, think about dropping, placing in a frame, etc. ")]
        public UnityEvent OnLose = new UnityEvent();
    }
}
