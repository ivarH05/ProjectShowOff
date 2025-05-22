using UnityEngine;

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
    }
}
