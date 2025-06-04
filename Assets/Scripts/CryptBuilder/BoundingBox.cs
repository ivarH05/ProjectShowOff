using System;
using Unity.VisualScripting;
using UnityEngine;

namespace CryptBuilder
{
    [Serializable]
    public struct BoundingBox : IEquatable<BoundingBox>
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
            return point.y <= Maximum.y;
        }

        public bool FullyContains(BoundingBox box) => ContainsPoint(box.Minimum) && ContainsPoint(box.Maximum);

        public bool Intersects(BoundingBox other)
        {
            return Minimum.x <= other.Maximum.x && Maximum.x >= other.Minimum.x &&
                Minimum.y <= other.Maximum.y && Maximum.y >= other.Minimum.y;
        }

        bool IEquatable<BoundingBox>.Equals(BoundingBox other)
        {
            return other.Minimum == this.Minimum && other.Maximum == this.Maximum;
        }

        public BoundingBox(Vector2 min, Vector2 max)
        {
            Minimum = min;
            Maximum = max;
        }

        public static bool operator== (BoundingBox left, BoundingBox right) { return (left as IEquatable<BoundingBox>).Equals(right); }
        public static bool operator!= (BoundingBox left, BoundingBox right) { return !(left == right); }
        public override bool Equals(object obj)
        {
            if(obj is BoundingBox b) return (this as IEquatable<BoundingBox>).Equals(b); 
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Minimum.GetHashCode(), Maximum.GetHashCode());
        }
    }
}