using UnityEngine;

namespace CryptBuilder
{
    public struct BoundingBox
    {
        public Vector2 Minimum;
        public Vector2 Maximum;

        public BoundingBox(Vector2 min, Vector2 max)
        {
            Minimum = min;
            Maximum = max;
        }
    }
}