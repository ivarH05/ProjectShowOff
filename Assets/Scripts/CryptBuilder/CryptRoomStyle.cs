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
        public float WallHeight;
        public WallDecoration ArchDecoration;
        public CornerDecoration PillarDecoration;

        [System.Serializable]
        public struct WallDecoration
        {
            public GameObject Prefab;
            public float Width;
            public bool FlipEveryOther;
        }

        [System.Serializable]
        public struct CornerDecoration
        {
            public GameObject Prefab;
        }
    }
}

