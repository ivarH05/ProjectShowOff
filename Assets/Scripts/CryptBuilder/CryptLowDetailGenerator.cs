using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;

namespace CryptBuilder
{

    public struct CryptLowDetailGenerator : Builder.ICryptSurfaceGenerator
    {
        public CryptRoomStyle DefaultStyle;
        
        CryptRoomStyle _currentStyle;
        RotatedRectangle _currentRoom;
        Vector2 _tileOffset;
        bool _validRoom;

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

            float priority = shape.Size.x * shape.Size.y;
            priority = Mathf.Exp(-.02f*priority);
            var quad = CreateQuad(_currentStyle.LowDetailFloor);
            quad.transform.localScale = new Vector3(shape.Size.x, shape.Size.y, 1);
            quad.transform.localPosition = Vector3.up * -.1f * priority;
            quad.transform.rotation = Quaternion.Euler(90,0,0);
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
            quad.transform.localScale = new Vector3(length, 4, 1);
            quad.transform.localPosition = new(center.x, 1, center.y);
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
        }
    }
}
