using UnityEngine;

namespace CryptBuilder
{

    public struct CryptLowDetailGenerator : Builder.ICryptSurfaceGenerator
    {
        public CryptRoomStyle DefaultStyle;
        
        CryptRoomStyle _currentStyle;
        RotatedRectangle _currentRoom;
        Vector2 _tileOffset;
        bool _validRoom;
        
        public void GenerateFloor(BoundingBox shape)
        {
            if (!_validRoom) return;

            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var obj = quad.GetComponent<Collider>();
#if UNITY_EDITOR
            Object.DestroyImmediate(obj);
#else
            Object.Destroy(obj);
#endif
            quad.transform.SetParent(_currentRoom.Room.GeneratedChildren.transform);
            quad.transform.localScale = new Vector3(shape.Size.x, shape.Size.y, 1);
            quad.transform.localPosition = default;
            quad.transform.rotation = Quaternion.Euler(90,0,0);
        }

        public void GenerateWall(Vector2 start, Vector2 end, Vector2 normal)
        {
            if (!_validRoom) return;
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
