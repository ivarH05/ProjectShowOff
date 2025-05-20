using UnityEngine;

namespace CryptBuilder
{
    public partial class Builder : MonoBehaviour
    {
        public BoundingNode RectangleTree {get; private set;}
        [SerializeField] RotatedRectangle _heldRectangle;
    }
}
