using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CryptBuilder
{
    [SelectionBase, ExecuteInEditMode]
    public class CryptRoom : MonoBehaviour
    {
        public CryptRoomStyle Style;
        [field:SerializeField, HideInInspector] public uint FlipWallDecorationMask { get; private set; }
        [field:SerializeField, HideInInspector] public uint DontGenerateWallDecMask { get; private set; }

        [NonSerialized] public LOD CurrentLOD;
        [NonSerialized] public int LastGeneratedNodeIndex;
        [NonSerialized] public int LastGeneratedRectIndex;
        [NonSerialized] public Builder LastBuilder;

        private void Awake()
        {
            GameObject gen = DisabledAtDistanceChildren;
        }
        public GameObject DisabledAtDistanceChildren
        {
            get
            {
                if(_disabledAtDistance == null)
                {
                    _disabledAtDistance = new("Disabled at distance");
                    _disabledAtDistance.transform.SetParent(transform, false);
                }
                return _disabledAtDistance;
            }
        }
        [SerializeField, HideInInspector] GameObject _disabledAtDistance;

        public GameObject GeneratedChildren
        {
            get
            {
                if(_generatedChildren == null)
                {
                    _generatedChildren = new GameObject("Generated children");
                    _generatedChildren.hideFlags = HideFlags.HideAndDontSave;
                    _generatedChildren.transform.SetParent(transform, false);
                }
                return _generatedChildren;
            }
        }
        [SerializeField, HideInInspector] GameObject _generatedChildren;

        public GameObject Colliders
        {
            get
            {
                if(_colliders == null)
                {
                    _colliders = new GameObject("Generated colliders");
                    _colliders.transform.SetParent(transform, false);
                }
                return _colliders;
            }
        }
        [SerializeField, HideInInspector] GameObject _colliders;

        private void OnDestroy()
        {
            if (_generatedChildren != null)
                DestroyImmediate(_generatedChildren);
        }

        public enum LOD
        {
            None, LowDetail, HighDetail
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(CryptRoom))]
        class CryptRoomEditor : Editor
        {
            int _selected = 0;
            int _wallCount;
            bool _cur;

            private void OnSceneGUI()
            {
                CryptRoom tgt = (CryptRoom)target;
                if (tgt.LastBuilder == null) 
                    return;

                GenSelectedWallGizmo giz = new();
                giz.Selected = _selected;
                giz.Flipped = _cur;
                tgt.LastBuilder.GenerateSurfaces(tgt.LastGeneratedNodeIndex, tgt.LastGeneratedRectIndex, ref giz);
                _wallCount = giz.WallIndex;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (_wallCount == 0) return;

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Selected wall: ");
                if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
                {
                    _selected--;
                    _selected += _wallCount;
                    _selected %= _wallCount;
                    SceneView.RepaintAll();
                }
                GUI.enabled = false;
                EditorGUILayout.IntField(_selected, GUILayout.MaxWidth(30));
                GUI.enabled = true;
                if (GUILayout.Button("+", GUILayout.MaxWidth(20)))
                {
                    _selected++;
                    _selected %= _wallCount;
                    SceneView.RepaintAll();
                }
                GUILayout.EndHorizontal();

                CryptRoom tgt = (CryptRoom)target;
                _cur = ((tgt.FlipWallDecorationMask >> _selected) & 1u ) != 0;
                bool generate = ((tgt.DontGenerateWallDecMask >> _selected) & 1u) == 0;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"Generate wall: ");
                var newGen = EditorGUILayout.Toggle(generate);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"Wall flipped: ");
                var newcur = EditorGUILayout.Toggle(_cur);
                EditorGUILayout.EndHorizontal();

                if (_cur != newcur)
                {
                    Undo.RecordObject(tgt, $"Flip wall {_selected} at room {tgt.LastGeneratedNodeIndex},{tgt.LastGeneratedRectIndex}");
                    uint mask = 1u << _selected;
                    tgt.FlipWallDecorationMask ^= mask;
                    _cur = newcur;
                    SceneView.RepaintAll();
                }
                if (generate != newGen)
                {
                    Undo.RecordObject(tgt, $"Set wall {_selected}'s generation at room {tgt.LastGeneratedNodeIndex},{tgt.LastGeneratedRectIndex}");
                    uint mask = 1u << _selected;
                    tgt.DontGenerateWallDecMask ^= mask;
                    SceneView.RepaintAll();
                }
            }
        }

        struct GenSelectedWallGizmo : Builder.ICryptSurfaceGenerator
        {
            public int Selected;
            public int WallIndex;
            public bool Flipped;

            Vector2 _surfaceOffset;
            bool _genArches;

            public void GenerateFloor(BoundingBox shape){}

            public void GenerateWall(Vector2 start, Vector2 end, Vector2 normal)
            {
                if(WallIndex == Selected)
                {
                    Handles.color = _genArches ? Color.green : Color.red;
                    start += _surfaceOffset;
                    end += _surfaceOffset;
                    Handles.DrawLine(start.To3D(), end.To3D());
                    Handles.DrawLine(start.To3D(3), end.To3D(3));
                    Handles.DrawLine(start.To3D(), start.To3D(3));
                    Handles.DrawLine(end.To3D(0), end.To3D(3));
                    if (Flipped) (start, end) = (end, start);
                    Handles.DrawLine(start.To3D(), end.To3D(3));
                }
                WallIndex++;
            }

            public void OnNewRoom(RotatedRectangle room)
            {
                WallIndex = 0;
                _surfaceOffset = room.Room.transform.position.To2D() - room.CenterPosition;
                _genArches = (room.Room.DontGenerateWallDecMask & (1 << Selected)) == 0;
            }
        }
#endif
    }
}
