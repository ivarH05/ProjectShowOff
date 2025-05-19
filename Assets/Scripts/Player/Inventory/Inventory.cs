using Player;
using Interactables;
using UnityEngine;

namespace Player.Inventory
{
    [RequireComponent(typeof(PlayerController))]
    public class InventoryManager : MonoBehaviour
    {
        public Slot[] slots;
    }

    public class Slot
    {

    }
}
