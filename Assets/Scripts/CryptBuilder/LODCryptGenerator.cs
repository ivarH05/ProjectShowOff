using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryptBuilder
{
    [ExecuteInEditMode]
    public class LODCryptGenerator : MonoBehaviour
    {
        [SerializeField] Builder _crypt;
        [SerializeField] float _lowDetailRange = 30;
        [SerializeField] float _highDetailRange = 10;
        [SerializeField] float _updateIntervalSeconds = .5f;

        List<(int, int)> _currentLowDetailRects = new();
        List<(int, int)> _currentHighDetailRects = new();
        float _updateTime = 0;

        private void Update()
        {
            _updateTime += Time.deltaTime;
            if(_updateTime > _updateIntervalSeconds)
            {
                UpdateLOD();
                _updateTime = 0;
            }
        }
        
        void SafeDestroy(Object obj)
        {
#if UNITY_EDITOR
            DestroyImmediate(obj);
#else
            Destroy(obj);
#endif
        }

        void UpdateLOD()
        {
            var oldLowDetail = _currentLowDetailRects;
            var oldHighDetail = _currentHighDetailRects;
            _currentLowDetailRects = GetRectanglesInRange(_lowDetailRange);
            _currentHighDetailRects = GetRectanglesInRange(_highDetailRange);

            foreach((int node, int rect) in oldLowDetail)
            {
                if(_currentLowDetailRects.Contains((node, rect)))
                    continue;

                var r = _crypt.RectangleTree.Nodes[node].Rectangles[rect];
                SafeDestroy(r.Room.GeneratedChildren);
                r.Room.CurrentLOD = CryptRoom.LOD.None;
            }
            var generatorH = new CryptHighDetailGenerator();
            generatorH.DefaultStyle = _crypt.DefaultStyle;
            var generatorL = new CryptLowDetailGenerator();
            generatorL.DefaultStyle = _crypt.DefaultStyle;

            foreach ((int node, int rect) in _currentLowDetailRects)
            {
                var r = _crypt.RectangleTree.Nodes[node].Rectangles[rect];
                bool highDetail = _currentHighDetailRects.Contains((node, rect));
                switch (r.Room.CurrentLOD)
                {
                    case CryptRoom.LOD.None:
                        if(!highDetail)
                        {
                            r.Room.CurrentLOD = CryptRoom.LOD.LowDetail;
                            SafeDestroy(r.Room.GeneratedChildren);
                            _crypt.GenerateSurfaces(node, rect, ref generatorL);
                        }
                        else
                        {
                            r.Room.CurrentLOD = CryptRoom.LOD.HighDetail;
                            SafeDestroy(r.Room.GeneratedChildren);
                            _crypt.GenerateTiles(node, rect, ref generatorH);
                        }
                        break;

                    case CryptRoom.LOD.HighDetail:
                        if (highDetail)
                            continue;
                        
                        r.Room.CurrentLOD = CryptRoom.LOD.LowDetail;
                        SafeDestroy(r.Room.GeneratedChildren);
                        _crypt.GenerateSurfaces(node, rect, ref generatorL);
                        break;

                    case CryptRoom.LOD.LowDetail:
                        if (!highDetail)
                            continue;

                        r.Room.CurrentLOD = CryptRoom.LOD.HighDetail;
                        SafeDestroy(r.Room.GeneratedChildren);
                        _crypt.GenerateTiles(node, rect, ref generatorH);
                        break;
                }
            }
        }

        List<(int, int)> GetRectanglesInRange(float range)
        {
            Vector2 pos = (transform.position - _crypt.transform.position).To2D();
            var off = new Vector2(range, range);
            var BB = new BoundingBox(pos - off, pos + off);

            return _crypt.RectangleTree.GetRectanglesIntersectingBox(BB);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(LODCryptGenerator))]
        class LODGenEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                GUILayout.Space(10);
                if(GUILayout.Button("Reset LOD cache (in case you modify the crypt)"))
                {
                    var c = (target as LODCryptGenerator);
                    c._currentLowDetailRects.Clear();
                    c._currentHighDetailRects.Clear();
                }
            }

            void OnSceneGUI()
            {
                var b = (LODCryptGenerator)target;
                var build = b._crypt;
                if (build == null) return;
                Handles.matrix = Matrix4x4.TRS(build.transform.position, Quaternion.identity, Vector3.one);

                Vector2 pos = (b.transform.position - b._crypt.transform.position).To2D();
                var off = new Vector2(b._lowDetailRange, b._lowDetailRange);
                var mouseBB = new BoundingBox(pos - off, pos + off);
                Handles.color = new(.4f, .7f, 0, .8f);
                CryptHandles.DrawBoundingBox(mouseBB);
                    
                var rects = build.RectangleTree.GetRectanglesIntersectingBox(mouseBB);
                foreach ((int nodeI, int rectI) in rects)
                {
                    CryptHandles.DrawBoundingBox(build.RectangleTree.Nodes[nodeI].Rectangles[rectI].GetBounds(), 1);
                }

                off = new Vector2(b._highDetailRange, b._highDetailRange);
                mouseBB = new BoundingBox(pos - off, pos + off);
                Handles.color = new(.1f, .6f, 0.8f, .8f);
                CryptHandles.DrawBoundingBox(mouseBB);

                rects = build.RectangleTree.GetRectanglesIntersectingBox(mouseBB);
                foreach ((int nodeI, int rectI) in rects)
                {
                    CryptHandles.DrawBoundingBox(build.RectangleTree.Nodes[nodeI].Rectangles[rectI].GetBounds(), 2);
                }
            }
        }
#endif
    }
}
