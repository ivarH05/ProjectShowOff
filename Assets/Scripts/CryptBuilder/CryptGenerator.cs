using UnityEngine;

namespace CryptBuilder
{
    public struct CryptGenerator : Builder.ICryptTileGenerator
    {
        public CryptRoomStyle DefaultStyle;

        CryptRoomStyle _currentStyle;
        GameObject _currentRoom;
        
        public void GenerateFloor(Vector2 point)
        {
            var prefabs = _currentStyle?.TilePrefabs;
            if (prefabs == null || prefabs.Length < 1) return;

            var floorInstance = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)], _currentRoom.transform);
            floorInstance.transform.position = point.To3D();
        }

        public void GenerateWall(Vector2 point, Vector2 normal)
        {
            var prefabs = _currentStyle?.WallPrefabs;
            if (prefabs == null || prefabs.Length < 1) return;

            var floorInstance = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)], _currentRoom.transform);
            floorInstance.transform.position = point.To3D();
            floorInstance.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(normal.y, -normal.x) * Mathf.Rad2Deg, Vector3.up);
        }

        public void OnNewRoom(RotatedRectangle room)
        {
            if (room.Room == null)
                _currentRoom = new GameObject($"Missing room object!");
            else _currentRoom = room.Room.GeneratedChildren;
            _currentStyle = room.Room?.Style == null ? DefaultStyle : room.Room.Style;
        }
    }
}
