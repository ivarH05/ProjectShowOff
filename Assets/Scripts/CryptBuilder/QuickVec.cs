using UnityEngine;

namespace CryptBuilder
{
    public static class QuickVec
    {
        public static Vector2 Min = new Vector2(float.MinValue, float.MinValue);
        public static Vector2 Max = new Vector2(float.MaxValue, float.MaxValue);
        public static Vector3 To3D(this Vector2 vec, float height = 0) => new(vec.x, height, vec.y);
        public static Vector2 To2D(this Vector3 vec) => new(vec.x, vec.z);
        public static Vector2 Round(this Vector2 vec)
        {
            vec.x = Mathf.Round(vec.x);
            vec.y = Mathf.Round(vec.y);
            return vec;
        }
    }
}
