using System.Collections.Generic;
using UnityEngine;

namespace CryptBuilder
{
    public struct CryptHighDetailGenerator : Builder.ICryptTileGenerator, Builder.ICryptSurfaceGenerator
    {
        public CryptRoomStyle DefaultStyle;
        public Builder Crypt;
        public List<(Vector3 start, Vector3 end, Color col)> Gizmos;

        CryptRoomStyle _currentStyle;
        Transform _nonRotatedGenChildren;
        RotatedRectangle _currentRoom;
        BoundingBox _currentBounds;
        Vector2 _tileOffset;
        Vector2 _surfaceOffset;
        Vector2 _previousWallNormal;
        bool _validRoom;
        int _currentWallIndex;
        
        public void GenerateFloor(Vector2 point)
        {
            if(!_validRoom) return;
            var prefabs = _currentStyle?.TilePrefabs;
            if (prefabs == null || prefabs.Length < 1) return;

            var floorInstance = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)], _currentRoom.Room.GeneratedChildren.transform);
            floorInstance.transform.position = (point + _tileOffset).To3D();
            floorInstance.transform.rotation = Quaternion.AngleAxis(90 * (int)(Random.value * 4), Vector3.up);
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
            if (_currentStyle.ArchDecoration.Prefab == null && _currentStyle.PillarDecoration.Prefab == null) return;

            WallPointType wallStart = WallTypeAtPoint(start);
            WallPointType wallEnd = WallTypeAtPoint(end);
            bool startIsCorner = wallStart >= WallPointType.InnerCorner;
            bool endIsCorner = wallEnd >= WallPointType.InnerCorner;

            start -= _surfaceOffset;
            end -= _surfaceOffset;
            float angle = Mathf.Atan2(normal.x, normal.y) * Mathf.Rad2Deg;
            float length = Vector2.Distance(start, end);
            bool alignedX = Mathf.Abs(normal.x) > .01f;
            
            if(alignedX && _currentStyle.PillarDecoration.Prefab != null)
            {
                GenPillar(start, wallStart, _currentStyle.PillarDecoration.Prefab, _nonRotatedGenChildren);
                GenPillar(end, wallEnd, _currentStyle.PillarDecoration.Prefab, _nonRotatedGenChildren);
            }
            void GenPillar(Vector2 pos, WallPointType type, GameObject prefab, Transform nonRotatedChildren)
            {
                if (!(type == WallPointType.OuterCorner || type == WallPointType.InnerCorner)) 
                    return;

                var decoInstance = Object.Instantiate(prefab, nonRotatedChildren);
                decoInstance.transform.localPosition = pos.To3D(0);
            }

            var detailCount = Mathf.Abs(length / _currentStyle.ArchDecoration.Width);
            bool integerDetailCount = Mathf.Abs(detailCount - (int)detailCount) < .01f;

            if (_currentStyle.ArchDecoration.Width < .02) return;
            if (_currentStyle.ArchDecoration.Prefab == null) return;
            bool genArches = (_currentRoom.Room.DontGenerateWallDecMask & (1 << _currentWallIndex)) == 0;
            if (!genArches || !(startIsCorner || endIsCorner || integerDetailCount)) 
                return; // cant really place arches here in any way

            Quaternion rotation = Quaternion.AngleAxis(angle+90, Vector3.up);
            Quaternion rotationFlipped = Quaternion.AngleAxis(angle+270, Vector3.up);
            Vector2 offset = (end-start).normalized * _currentStyle.ArchDecoration.Width;
            Vector2 placeStart;
            int count = (int)detailCount;
            if (integerDetailCount)
            {
                placeStart = start;
            }
            else if (endIsCorner)
            {
                placeStart = start;
                count++;
            }
            else
            {
                placeStart = end;
                offset = -offset;
                count++;
            }
            if(alignedX && !integerDetailCount)
                alignedX = false;

            bool flipflip = (_currentRoom.Room.FlipWallDecorationMask & (1 << _currentWallIndex)) != 0;
            for (int i = 0; i < count; i++)
            {
                bool flipped = ((i & 1) == 0) ^ flipflip & _currentStyle.ArchDecoration.FlipEveryOther;
                Vector2 pos = placeStart + offset * (i+1 - (flipped^alignedX ? 0 : 1));
                var decoInstance = Object.Instantiate(_currentStyle.ArchDecoration.Prefab, _nonRotatedGenChildren);
                decoInstance.transform.localPosition = pos.To3D(0);
                decoInstance.transform.rotation = flipped ? rotation : rotationFlipped;
            }

            _currentWallIndex++;
        }
        WallPointType WallTypeAtPoint(Vector2 v)
        {
            int wallHits = 0;
            float size = Crypt.RectRounding * .5f;
            
            if (!Crypt.RectangleTree.TryGetRectangleAtPoint(v + new Vector2(-size, -size), out _, out _))
                wallHits++;
            if (!Crypt.RectangleTree.TryGetRectangleAtPoint(v + new Vector2(-size, size), out _, out _))
                wallHits++;
            if (!Crypt.RectangleTree.TryGetRectangleAtPoint(v + new Vector2(size, -size), out _, out _))
                wallHits++;
            if (!Crypt.RectangleTree.TryGetRectangleAtPoint(v + new Vector2(size, size), out _, out _))
                wallHits++;
            
            return (WallPointType)wallHits;
        }
        enum WallPointType
        {
            NoWalls = 0,
            OuterCorner = 1,
            Wall = 2,
            InnerCorner = 3,
            FullyInWall = 4
        }

        public void OnNewRoom(RotatedRectangle room)
        {
            _currentWallIndex = 0;
            _validRoom = room.Room != null;
            if (!_validRoom) 
                Debug.LogError("Room is missing a generated room object! CONSULT WITH CAPS.");
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
                return;
            }

            var prefabs = _currentStyle?.WallPrefabs;
            if (prefabs == null || prefabs.Length < 1)
                Debug.LogWarning("Style is missing wall prefabs", _currentStyle);

            prefabs = _currentStyle?.TilePrefabs;
            if (prefabs == null || prefabs.Length < 1)
                Debug.LogWarning("Style is missing tile prefabs", _currentStyle);
        }
    }
}
