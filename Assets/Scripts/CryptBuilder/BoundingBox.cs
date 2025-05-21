using System;
using UnityEngine;

namespace CryptBuilder
{
    [Serializable]
    public struct BoundingBox
    {
        public Vector2 Minimum;
        public Vector2 Maximum;

        public Vector2 Center => (Minimum + Maximum) * .5f;
        public Vector2 Size => Maximum - Minimum;

        public bool IsValid => Size.x > 0 && Size.y > 0;

        /// <summary>
        /// Reshapes the bounding box to include a certain point, if it lies outside of the box.
        /// </summary>
        public void GrowToInclude(in Vector2 point)
        {
            Minimum = Vector2.Min(point, Minimum);
            Maximum = Vector2.Max(point, Maximum);
        }

        public void GrowToInclude(in BoundingBox box)
        {
            GrowToInclude(box.Minimum);
            GrowToInclude(box.Maximum);
        }

        public bool ContainsPoint(Vector2 point)
        {
            if(point.x < Minimum.x) return false;
            if(point.x > Maximum.x) return false;
            if(point.y < Minimum.y) return false;
            return point.y < Maximum.y;
        }

        public bool FullyContains(BoundingBox box) => ContainsPoint(box.Minimum) && ContainsPoint(box.Maximum);

        public bool Intersects(BoundingBox other)
        {
            if(ContainsPoint(other.Maximum) || ContainsPoint(other.Minimum)) return true;
            return other.ContainsPoint(Minimum) || other.ContainsPoint(Maximum);
        }

        public BoundingBox(Vector2 min, Vector2 max)
        {
            Minimum = min;
            Maximum = max;
        }
    }
}