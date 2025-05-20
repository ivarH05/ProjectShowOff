using UnityEngine;

namespace CryptBuilder
{
    public struct Matrix2x2
    {
        public Vector2 iHat;
        public Vector2 jHat;

        public Matrix2x2(Vector2 i, Vector2 j)
        {
            this.iHat = i;
            this.jHat = j;
        }

        public static Vector2 operator *(Matrix2x2 m, Vector2 v)
        {
            return v.x * m.iHat + v.y * m.jHat;
        }
        public static Vector2 operator *(Vector2 v, Matrix2x2 m)
        {
            return v.x * m.iHat + v.y * m.jHat;
        }
        public static Matrix2x2 fromRotationAngleRad(float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float s = Mathf.Sin(radians);
            float c = Mathf.Cos(radians);
            Vector2 i = new Vector2(c, -s);
            Vector2 j = new Vector2(s, c);
            return new (i, j);
        }
    }
}