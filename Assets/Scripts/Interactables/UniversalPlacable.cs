using Interactables;
using Player.InventoryManagement;
using UnityEngine;

public class UniversalPlacable : Placable
{
    private GameObject _placedStateObject;

    [SerializeField] private Vector3 offset;

    internal override void Place(Item item)
    {
        _placedStateObject = Instantiate(item.DefaultWorldObjectPrefab, transform);
        _placedStateObject.transform.localPosition = item.ItemFrameOffset + offset;
        ItemObject io = _placedStateObject.GetComponent<ItemObject>();
        if (io != null)
            Destroy(io);
        Rigidbody rb = _placedStateObject.GetComponent<Rigidbody>();
        if (rb != null)
            Destroy(rb);
        _placedStateObject.layer = 0;
    }

    internal override void Remove(Item item)
    {
        Destroy(_placedStateObject);
    }
}
