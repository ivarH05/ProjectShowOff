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
        /// Directly stolen from //https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection#Given_two_points_on_each_line_segment.
        /// </summary>
        /// <param name="other">The line to intersect with.</param>
        /// <returns>The T of intersection. The point can be found with Lerp(A,B,T)</returns>
        public float TIntersection(LineSegment other)
        {
            return ((A.x - other.A.x) * (other.A.y - other.B.y) - (A.y - other.A.y) * (other.A.x - other.B.x))
                / ((A.x - B.x) * (other.A.y - other.B.y) - (A.y - B.y) * (other.A.x - other.B.x));
        }

        public static LineSegment operator* (LineSegment l, Matrix2x2 m) => new(l.A * m,  l.B * m);
    }
}