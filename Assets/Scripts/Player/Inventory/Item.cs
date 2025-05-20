using UnityEngine;

namespace Player.InventoryManagement
{
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    public class Item : ScriptableObject
    {
        public GameObject Prefab;
        public Vector3 baseOffset;
        public Vector3 baseEulerAngles;
    }
}
