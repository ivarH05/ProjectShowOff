using System;
using System.Collections.Generic;
using UnityEngine;

namespace CryptBuilder
{
    [Serializable]
    public struct RotatedRectangle : IEquatable<RotatedRectangle>
    {
        public Vector2 HalfSize;
        public Vector2 CenterPosition;
        public float Rotation;

        public bool IsValid => HalfSize.x > 0 && HalfSize.y > 0;

        /// <summary>
        /// Tests wether a point lies within the rotated rectangle.
        /// </summary>
        public bool ContainsPoint(Vector2 point)
        {
            point -= CenterPosition;
            if(Rotation != 0)
            {
                Matrix2x2 rotation = Matrix2x2.FromRotationAngle(-Rotation);
                point *= rotation;
            }
            return Mathf.Abs(point.x) < HalfSize.x && Mathf.Abs(point.y) < HalfSize.y;
        }

        /// <summary>
        /// Gets each line that makes up the rectangle.
        /// </summary>
        public IEnumerable<LineSegment> GetLines()
        {
            Matrix2x2 rotation = Matrix2x2.FromRotationAngle(Rotation);
            Vector2 forward = rotation.iHat * HalfSize.x;
            Vector2 right = rotation.jHat * HalfSize.y;
            Vector2 corner00 = CenterPosition - forward - right;
            Vector2 corner01 = CenterPosition + forward - right;
            yield return new LineSegment(corner00, corner01);
            Vector2 corner11 = CenterPosition + forward + right;
            yield return new LineSegment(corner01, corner11);
            Vector2 corner10 = CenterPosition - forward + right;
            yield return new LineSegment(corner11, corner10);
            yield return new LineSegment(corner10, corner00);
        }

        /// <summary>
        /// Gets a bounding box that fits the rotated rectangle.
        /// </summary>
        public BoundingBox GetBounds()
        {
            if (Rotation == 0) 
                return new BoundingBox(CenterPosition - HalfSize, CenterPosition + HalfSize); // for free

            Matrix2x2 rotation = Matrix2x2.FromRotationAngle(Rotation);
            Vector2 forward = rotation.iHat * HalfSize.x;
            Vector2 right = rotation.jHat * HalfSize.y;
            Vector2 corner00 = CenterPosition - forward - right;
            Vector2 corner01 = CenterPosition + forward - right;
            Vector2 corner11 = CenterPosition + forward + right;
            Vector2 corner10 = CenterPosition - forward + right;

            Vector2 max = Vector2.Max( Vector2.Max(corner00, corner01), Vector2.Max(corner10, corner11));
            Vector2 min = Vector2.Min( Vector2.Min(corner00, corner01), Vector2.Min(corner10, corner11));
            return new BoundingBox(min, max);
        }

        bool IEquatable<RotatedRectangle>.Equals(RotatedRectangle other)
        {
            return Rotation == other.Rotation && CenterPosition == other.CenterPosition && HalfSize == other.HalfSize;
        }
        public override bool Equals(object obj)
        {
            if(obj is BoundingBox b) return Equals(b); 
            return false;
        }
        public static bool operator== (RotatedRectangle b1, RotatedRectangle b2) { return (b1 as IEquatable<RotatedRectangle>).Equals(b2); }
        public static bool operator!= (RotatedRectangle b1, RotatedRectangle b2) => !(b1 == b2);
        public override int GetHashCode()
        {
            return HashCode.Combine(Rotation, CenterPosition, HalfSize);
        }
    }
}
