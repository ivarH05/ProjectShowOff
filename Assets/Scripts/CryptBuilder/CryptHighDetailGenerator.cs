using UnityEngine;

namespace CryptBuilder
{
    public struct CryptHighDetailGenerator : Builder.ICryptTileGenerator, Builder.ICryptSurfaceGenerator
    {
        public CryptRoomStyle DefaultStyle;

        CryptRoomStyle _currentStyle;
        Transform _nonRotatedGenChildren;
        RotatedRectangle _currentRoom;
        BoundingBox _currentBounds;
        Vector2 _tileOffset;
        Vector2 _surfaceOffset;
        Vector2 _previousWallNormal;
        bool _validRoom;
        
        public void GenerateFloor(Vector2 point)
        {
            if(!_validRoom) return;
            var prefabs = _currentStyle?.TilePrefabs;
            if (prefabs == null || prefabs.Length < 1) return;

            var floorInstance = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)], _currentRoom.Room.GeneratedChildren.transform);
            floorInstance.transform.position = (point + _tileOffset).To3D();
        }

        public void GenerateFloor(BoundingBox shape){}

        public void GenerateWall(Vector2 point, Vector2 normal)
        {
            if(!_validRoom) return;
            var prefabs = _currentStyle?.WallPrefabs;
            if (prefabs == null || prefabs.Length < 1) return;

            var floorInstance = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)], _currentRoom.Room.GeneratedChildren.transform);
            floorInstance.transform.position = (point + _tileOffset).To3D();
            floorInstance.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(normal.y, -normal.x) * Mathf.Rad2Deg, Vector3.up);
        }

        public void GenerateWall(Vector2 start, Vector2 end, Vector2 normal)
        {
            if(!_validRoom) return;
            if (_currentStyle.Decoration.Prefab == null) return;
            if (!(_currentStyle.Decoration.Width > .02)) return;

            bool startIsCorner = IsCorner(start);
            bool endIsCorner = IsCorner(end);
            if(startIsCorner && endIsCorner)
            {
                startIsCorner = false;
                endIsCorner = false;
            }

            start -= _surfaceOffset;
            end -= _surfaceOffset;
            float angle = Mathf.Atan2(normal.x, normal.y) * Mathf.Rad2Deg;
            float length = Vector2.Distance(start, end);

            var detailCount = Mathf.Abs(length / _currentStyle.Decoration.Width);
            bool integerDetailCount = Mathf.Abs(detailCount - (int)detailCount) < .01f;

            if (!(startIsCorner || endIsCorner || integerDetailCount)) 
                return; // cant really place walls here in any way

            Quaternion rotation = Quaternion.AngleAxis(angle+90, Vector3.up);
            Quaternion rotationFlipped = Quaternion.AngleAxis(angle+270, Vector3.up);
            Vector2 offset = (end-start).normalized * _currentStyle.Decoration.Width;
            Vector2 placeStart;
            float height = 0;
            int count = (int)detailCount;
            bool alignedX = Mathf.Abs(normal.x) > .01f;
            if (integerDetailCount)
            {
                placeStart = start;
                //height = 0f;
            }
            else if (endIsCorner)
            {
                placeStart = start;
                //height = 0.5f;
                count++;
            }
            else
            {
                placeStart = end;
                offset = -offset;
                //height = 1f;
                count++;
            }
            if(alignedX && !integerDetailCount)
                alignedX = false;

            for (int i = 0; i < count; i++)
            {
                bool flipped = ((i & 1) == 0) & _currentStyle.Decoration.FlipEveryOther;
                Vector2 pos = placeStart + offset * (i+1 - (flipped^alignedX ? 0 : 1));
                var decoInstance = Object.Instantiate(_currentStyle.Decoration.Prefab, _nonRotatedGenChildren);
                decoInstance.transform.localPosition = pos.To3D(height);
                decoInstance.transform.rotation = flipped ? rotation : rotationFlipped;
            }
        }
        bool IsCorner(Vector2 v)
        {
            var bounds = _currentBounds;
            var size = bounds.Size.magnitude * .1f;
            if (Vector2.Distance(bounds.Minimum, v) < size) return true;
            if (Vector2.Distance(bounds.Maximum, v) < size) return true;
            if (Vector2.Distance(new(bounds.Minimum.x, bounds.Maximum.y), v) < size) return true;
            if (Vector2.Distance(new(bounds.Maximum.x, bounds.Minimum.y), v) < size) return true;
            return false;
        }

        public void OnNewRoom(RotatedRectangle room)
        {
            _validRoom = room.Room != null;
            if (!_validRoom) 
                Debug.LogError("Room is missing a generated room object! Click \"Regenerate rooms\" on the cryptbuilder.");
            else
            {
                _tileOffset = room.Room.transform.position.To2D() - room.CenterPosition;
                _surfaceOffset = room.CenterPosition;
                var nonRotated = new GameObject("NonRotated");
                nonRotated.transform.SetParent(room.Room.GeneratedChildren.transform, false);
                nonRotated.transform.rotation = Quaternion.identity;
                _nonRotatedGenChildren = nonRotated.transform;
            }

            _currentRoom = room;
            _currentStyle = room.Room?.Style == null ? DefaultStyle : room.Room.Style;
            _currentBounds = room.GetBounds();
            if(_currentStyle == null)
            {
                _validRoom = false;
                Debug.LogError("Room has no style, and the crypt doesnt have a default style!");
            }

            var prefabs = _currentStyle?.WallPrefabs;
            if (prefabs == null || prefabs.Length < 1)
                Debug.LogError("missing wall prefabs");

            prefabs = _currentStyle?.TilePrefabs;
            if (prefabs == null || prefabs.Length < 1)
                Debug.LogError("missing tile prefabs");
        }
    }
}
