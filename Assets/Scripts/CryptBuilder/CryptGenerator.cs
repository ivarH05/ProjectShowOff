using UnityEngine;

namespace CryptBuilder
{
    public struct CryptGenerator : Builder.ICryptGenerator
    {
        public GameObject CryptRoot;
        public CryptRoomStyle DefaultStyle;

        CryptRoomStyle _currentStyle;
        GameObject _currentRoom;
        int _roomCount;
        
        public void GenerateFloor(Vector2 point)
        {
            var prefabs = _currentStyle?.TilePrefabs;
            if (prefabs == null || prefabs.Length < 1) return;

            var floorInstance = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)], _currentRoom.transform);
            floorInstance.transform.localPosition = point.To3D();
        }

        public void GenerateWall(Vector2 point, Vector2 normal)
        {
            var prefabs = _currentStyle?.WallPrefabs;
            if (prefabs == null || prefabs.Length < 1) return;

            var floorInstance = Object.Instantiate(
                prefabs[Random.Range(0, prefabs.Length)], 
                default, 
                Quaternion.AngleAxis(Mathf.Atan2(normal.y, -normal.x) * Mathf.Rad2Deg, Vector3.up), 
                _currentRoom.transform);
            floorInstance.transform.localPosition = point.To3D();
        }

        public void OnNewRoom(RotatedRectangle room)
        {
            _currentRoom = new GameObject($"Room {_roomCount}");
            _currentRoom.transform.SetParent(CryptRoot.transform, false);
            _currentStyle = room.Style == null ? DefaultStyle : room.Style;
            _roomCount++;
        }
    }
}
