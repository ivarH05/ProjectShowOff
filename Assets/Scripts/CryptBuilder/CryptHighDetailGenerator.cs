using UnityEngine;

namespace CryptBuilder
{
    public struct CryptHighDetailGenerator : Builder.ICryptTileGenerator
    {
        public CryptRoomStyle DefaultStyle;

        CryptRoomStyle _currentStyle;
        RotatedRectangle _currentRoom;
        Vector2 _tileOffset;
        bool _validRoom;
        
        public void GenerateFloor(Vector2 point)
        {
            if(!_validRoom) return;
            var prefabs = _currentStyle?.TilePrefabs;
            if (prefabs == null || prefabs.Length < 1) return;

            var floorInstance = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)], _currentRoom.Room.GeneratedChildren.transform);
            floorInstance.transform.position = (point + _tileOffset).To3D();
        }

        public void GenerateWall(Vector2 point, Vector2 normal)
        {
            if(!_validRoom) return;
            var prefabs = _currentStyle?.WallPrefabs;
            if (prefabs == null || prefabs.Length < 1) return;

            var floorInstance = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)], _currentRoom.Room.GeneratedChildren.transform);
            floorInstance.transform.position = (point + _tileOffset).To3D();
            floorInstance.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(normal.y, -normal.x) * Mathf.Rad2Deg, Vector3.up);
        }

        public void OnNewRoom(RotatedRectangle room)
        {
            _validRoom = room.Room != null;
            if (!_validRoom) 
                Debug.LogError("Room is missing a generated room object! Click \"Regenerate rooms\" on the cryptbuilder.");
            else
            {
                _tileOffset = room.Room.transform.position.To2D() - room.CenterPosition;
            }
            _currentRoom = room;
            _currentStyle = room.Room?.Style == null ? DefaultStyle : room.Room.Style;
        }
    }
}
