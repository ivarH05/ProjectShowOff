using UnityEngine;

namespace CryptBuilder
{
    public static class QuickVec
    {
        public static Vector2 Min = new Vector2(float.MinValue, float.MinValue);
        public static Vector2 Max = new Vector2(float.MaxValue, float.MaxValue);
        public static Vector3 To3D(this Vector2 vec, float height = 0) => new(vec.x, height, vec.y);
    }
}
