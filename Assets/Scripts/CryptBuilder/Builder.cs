using UnityEngine;

namespace CryptBuilder
{
    public partial class Builder : MonoBehaviour
    {
        [field:SerializeField] public RectangleCollection RectangleTree {get; private set;}
        [SerializeField] RotatedRectangle _heldRectangle;
    }
}
