using UnityEngine;

namespace CryptBuilder
{
    [CreateAssetMenu(fileName = "CryptRoomStyle", menuName = "Crypt room style")]
    public class CryptRoomStyle : ScriptableObject
    {
        public GameObject TilePrefab;
        public GameObject WallPrefab;
    }
}

