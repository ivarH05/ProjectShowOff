using UnityEngine;

namespace CryptBuilder
{
    public partial class Builder : MonoBehaviour
    {
        [field:SerializeField] public BoundingNode RectangleTree {get; private set;}
        [SerializeField] RotatedRectangle _heldRectangle;
    }
}
