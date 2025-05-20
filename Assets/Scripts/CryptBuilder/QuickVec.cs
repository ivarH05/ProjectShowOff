using UnityEngine;

namespace CryptBuilder
{
    public static class QuickVec
    {
        public static Vector3 To3D(this Vector2 vec, float height = 0) => new(vec.x, height, vec.y);
    }
}
