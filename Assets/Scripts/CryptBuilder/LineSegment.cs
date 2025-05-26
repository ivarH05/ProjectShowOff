using System;
using UnityEngine;

namespace CryptBuilder
{ 
    public struct LineSegment
    {
        public Vector2 A;
        public Vector2 B;

        public LineSegment(Vector2 a, Vector2 b)
        {
            A = a; 
            B = b; 
        }

        public Vector2 Normal
        {
            get
            {
                Vector2 dir = Direction;
                (dir.x, dir.y) = (dir.y, -dir.x); // they call me the swappinator
                return dir;
            }
        }
        public Vector2 Direction => (B - A).normalized;


        /// <summary>
        /// Gets the length of the line.
        /// </summary>
        public float Length() => Vector2.Distance(A, B);

        public float LengthSquared()
        {
            var off = B - A;
            return Vector2.Dot(off, off);
        }

        /// <summary>
        /// Tries to get the intersection time between two lines.
        /// </summary>
        /// <param name="other">The other line.</param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool TIntersection(LineSegment other, out float t)
        {
            // Directly stolen from https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection#Given_two_points_on_each_line_segment
            float divided = (A.x - other.A.x) * (other.A.y - other.B.y) - (A.y - other.A.y) * (other.A.x - other.B.x);
            float divisor = (A.x - B.x) * (other.A.y - other.B.y) - (A.y - B.y) * (other.A.x - other.B.x);
            t = divided / divisor;
            return t > 0 && t < 1;
        }

        public static LineSegment operator* (LineSegment l, Matrix2x2 m) => new(l.A * m,  l.B * m);

        public override string ToString()
        {
            return $"Line {A}, {B}";
        }
    }
}