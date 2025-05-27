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
            if (_currentStyle?.TilePrefab == null) return;

            var floorInstance = Object.Instantiate(_currentStyle.TilePrefab, _currentRoom.transform);
            floorInstance.transform.localPosition = point.To3D();
        }

        public void GenerateWall(Vector2 point, Vector2 normal)
        {
            if (_currentStyle?.WallPrefab == null) return;

            var floorInstance = Object.Instantiate(
                _currentStyle.WallPrefab, 
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
