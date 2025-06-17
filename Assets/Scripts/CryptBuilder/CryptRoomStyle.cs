using UnityEngine;

namespace CryptBuilder
{
    [CreateAssetMenu(fileName = "CryptRoomStyle", menuName = "Crypt room style")]
    public class CryptRoomStyle : ScriptableObject
    {
        public GameObject[] TilePrefabs;
        public GameObject[] WallPrefabs;
        public GameObject[] CeilingPrefabs;
        public Material LowDetailFloor;
        public Material LowDetailCeiling;
        public Material LowDetailWall;
    }
}

