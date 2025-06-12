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
        float _priority;
        float _roomHeight;

        GameObject CreateQuad(Material mat)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var obj = quad.GetComponent<Collider>();
#if UNITY_EDITOR
            Object.DestroyImmediate(obj);
#else
            Object.Destroy(obj);
#endif
            quad.transform.SetParent(_currentRoom.Room.GeneratedChildren.transform);
            if(mat != null)
                quad.GetComponent<Renderer>().material = mat;
            return quad;
        }

        public void GenerateFloor(BoundingBox shape)
        {
            if (!_validRoom) return;

            var quad = CreateQuad(_currentStyle.LowDetailFloor);
            quad.transform.localScale = new Vector3(shape.Size.x, shape.Size.y, 1);
            quad.transform.localPosition = Vector3.up * -_priority;
            quad.transform.rotation = Quaternion.Euler(90,0,0);

            var quad2 = CreateQuad(_currentStyle.LowDetailCeiling);
            quad2.transform.localScale = new Vector3(shape.Size.x, shape.Size.y, 1);
            quad2.transform.localPosition = Vector3.up * (_priority + _roomHeight);
            quad2.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }

        public void GenerateWall(Vector2 start, Vector2 end, Vector2 normal)
        {
            if (!_validRoom) return;

            start -= _tileOffset;
            end -= _tileOffset;
            float angle = Mathf.Atan2(normal.x, normal.y) * Mathf.Rad2Deg;
            float length = Vector2.Distance(start, end);
            Vector2 center = .5f * (start + end);

            var quad = CreateQuad(_currentStyle.LowDetailWall);
            quad.transform.localScale = new Vector3(length, _roomHeight + 2*_priority, 1);
            quad.transform.localPosition = new(center.x, .5f* _roomHeight, center.y);
            quad.transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        public void OnNewRoom(RotatedRectangle room)
        {
            _validRoom = room.Room != null;
            if (!_validRoom)
                Debug.LogError("Room is missing a generated room object! Click \"Regenerate rooms\" on the cryptbuilder.");
            else
            {
                _tileOffset = room.CenterPosition;
                room.Room.GeneratedChildren.transform.rotation = Quaternion.identity;
            }
            _currentRoom = room;
            _currentStyle = room.Room?.Style == null ? DefaultStyle : room.Room.Style;
            if (_currentStyle == null)
            {
                _validRoom = false;
                return;
            }

            var shape = room.GetBounds();
            _priority = shape.Size.x * shape.Size.y;
            _priority = Mathf.Exp(-.02f * _priority) * .01f;

            _roomHeight = _currentStyle.WallHeight != 0 ? _currentStyle.WallHeight : 2;
        }
    }
}
