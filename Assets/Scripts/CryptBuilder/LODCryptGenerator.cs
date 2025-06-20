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
        List<(Vector3 start, Vector3 end, Color col)> _gizmos;
        float _updateTime = 0;
        int _updates = 0;

        private void Awake()
        {
            Debug.developerConsoleEnabled = true;
            Debug.developerConsoleVisible = true;
            if(_crypt == null)
            {
                Debug.LogError("LODCryptGenerator wants to have a crypt assigned! It will not generate anything if this is missing.");
            }
            _currentHighDetailRects.Clear();
            _currentLowDetailRects.Clear();
            _updates = 0;
        }

        private void Update()
        {
            _updateTime += Time.deltaTime;
            if(_updateTime > _updateIntervalSeconds || _updates < 2)
            {
                try
                {
                    UpdateLOD();
                }
                catch(System.Exception)
                {
                    // something went wrong with generation, so clear the cache
                    _currentHighDetailRects.Clear();
                    _currentLowDetailRects.Clear();
                }
                _updateTime = 0;
            }
        }
        
        void UpdateLOD()
        {
            if (_crypt == null) return;

            _updates++;
            var oldLowDetail = _currentLowDetailRects;
            var oldHighDetail = _currentHighDetailRects;
            _currentLowDetailRects = GetRectanglesInRange(_lowDetailRange);
            _currentHighDetailRects = GetRectanglesInRange(_highDetailRange);

            foreach((int node, int rect) in oldLowDetail)
            {
                if(_currentLowDetailRects.Contains((node, rect)))
                    continue;

                // destroy rooms out of range
                var r = _crypt.RectangleTree.Nodes[node].Rectangles[rect];
                DestroyImmediate(r.Room.GeneratedChildren);
                r.Room.CurrentLOD = CryptRoom.LOD.None;
            }
            var generatorH = new CryptHighDetailGenerator();
            generatorH.Crypt = _crypt;
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
                            // create low detail room
                            r.Room.CurrentLOD = CryptRoom.LOD.LowDetail;
                            DestroyImmediate(r.Room.GeneratedChildren);
                            _crypt.GenerateSurfaces(node, rect, ref generatorL);
                        }
                        else
                        {
                            // create high detail room
                            r.Room.CurrentLOD = CryptRoom.LOD.HighDetail;
                            DestroyImmediate(r.Room.GeneratedChildren);
                            _crypt.GenerateTiles(node, rect, ref generatorH);
                            _crypt.GenerateSurfaces(node, rect, ref generatorH);
                        }
                        break;

                    case CryptRoom.LOD.HighDetail:
                        if (highDetail)
                            continue;
                        
                        // downgrade detail
                        r.Room.CurrentLOD = CryptRoom.LOD.LowDetail;
                        DestroyImmediate(r.Room.GeneratedChildren);
                        _crypt.GenerateSurfaces(node, rect, ref generatorL);
                        break;

                    case CryptRoom.LOD.LowDetail:
                        if (!highDetail)
                            continue;

                        // upgrade detail
                        r.Room.CurrentLOD = CryptRoom.LOD.HighDetail;
                        DestroyImmediate(r.Room.GeneratedChildren);
                        _crypt.GenerateTiles(node, rect, ref generatorH);
                        _crypt.GenerateSurfaces(node, rect, ref generatorH);
                        break;
                }
            }

            _gizmos ??= new();
            foreach(var r in generatorH.Gizmos)
                _gizmos.Add(r);
        }

        private void OnDrawGizmos()
        {
            if(_gizmos != null)
                foreach(var g in _gizmos)
                {
                    Gizmos.color = g.col;
                    Gizmos.DrawLine(g.end, g.start);    
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
