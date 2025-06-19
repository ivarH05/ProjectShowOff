#if UNITY_EDITOR
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace CryptBuilder
{
    public partial class Builder
    {
        [SerializeField, HideInInspector] EditMode _editMode;
        [SerializeField, HideInInspector] List<RotatedRectangle> _heldRectangles = new();
        List<RotatedRectangle> _rectangleClipboard = new();
        bool _debugShowBounds;
        CryptRoomStyle _brushStyle;

        [CustomEditor(typeof(Builder))]
        class BuilderEditor : Editor
        {
            private void OnDisable()
            {
                var b = (Builder)target;
                if (b == null) return;
                if(b._heldRectangles.Count > 0)
                {
                    DeselectHeld(b);
                }
            }
            static void CreateRectangleGameObject(ref RotatedRectangle rect, Builder b)
            {
                GameObject room = new GameObject("Room", typeof(CryptRoom));
                room.transform.SetParent(b.transform);
                room.transform.position = rect.CenterPosition.To3D() + b.transform.position;
                room.transform.rotation = Quaternion.Euler(0, rect.Rotation, 0);
                rect.Room = room.GetComponent<CryptRoom>();
                Undo.RegisterCreatedObjectUndo(room, "[CryptGenerator] Created room object");
            }
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                var b = (Builder)target;
                CryptHandles.RoundRectangleSize = b._rectRounding;
                CryptHandles.RoundRectangleRotation = b._rectRotationRounding;

                if(b._heldRectangles.Count == 1)
                {
                    var rectCurrent = b._heldRectangles[0];
                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Selected room's style:");
                    var newStyle = EditorGUILayout.ObjectField(rectCurrent.Room?.Style, typeof(CryptRoomStyle), false);
                    EditorGUILayout.EndHorizontal();

                    if (newStyle != rectCurrent.Room.Style)
                    {
                        Undo.RecordObject(b, "[CryptBuilder] Set selected room's style");
                        rectCurrent.Room.Style = (CryptRoomStyle)newStyle;
                        b._heldRectangles[0] = rectCurrent;
                        EditorUtility.SetDirty(b);
                    }
                }

                GUILayout.Label("Tools");
                if(GUILayout.Button("Add new rect"))
                {
                    DeselectHeld(b);
                    b._editMode = EditMode.AddNew;
                }
                if (b._editMode != EditMode.StyleBrush)
                {
                    if (GUILayout.Button("Style brush"))
                    {
                        DeselectHeld(b);
                        b._editMode = EditMode.StyleBrush;
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Brush style:");
                    b._brushStyle = (CryptRoomStyle)EditorGUILayout.ObjectField(b._brushStyle, typeof(CryptRoomStyle), false);
                    EditorGUILayout.EndHorizontal();

                    if (GUILayout.Button("Disable style brush"))
                        b._editMode = default;
                }
                GUILayout.Label("Debug");
                if(GUILayout.Button("Toggle debug bounds"))
                {
                    b._debugShowBounds = !b._debugShowBounds;
                    SceneView.RepaintAll() ;
                }
                if(GUILayout.Button("Toggle show walls (may improve performance)"))
                {
                    b._showWallGizmos = !b._showWallGizmos;
                    SceneView.RepaintAll() ;
                }
                if (GUILayout.Button("Regenerate tree (may improve performance, fixes rounding)"))
                {
                    Undo.RecordObject(b, "[CryptBuilder] Regenerate tree");
                    b.RectangleTree.Regenerate(b._rectRounding, b._rectRotationRounding);
                    SceneView.RepaintAll();
                    EditorUtility.SetDirty(b);
                }
                GUILayout.Label("Danger zone");
                if(GUILayout.Button("(Re)generate hitboxes"))
                {
                    Undo.RegisterFullObjectHierarchyUndo(b.gameObject, "[CryptBuilder] Generate hitboxes");
                    HitboxGenerator gen = new();
                    gen.DefaultStyle = b._defaultStyle;
                    gen.TileScale = b._rectRounding;
                    b.GenerateSurfaces(gen);
                    EditorUtility.SetDirty(b);
                }
                if(GUILayout.Button("Clear hitboxes"))
                {
                    Undo.RegisterFullObjectHierarchyUndo(b.gameObject, "[CryptBuilder] Delete hitboxes");
                    foreach (var node in b.RectangleTree.Nodes)
                    {
                        var rectses = node.Rectangles;
                        if (rectses == null) continue;
                        foreach (var r in rectses)
                            Undo.DestroyObjectImmediate(r.Room.Colliders);
                    }
                    EditorUtility.SetDirty(b);
                }
                if(GUILayout.Button("Regenerate rooms"))
                {
                    Undo.RegisterFullObjectHierarchyUndo(b.gameObject, "[CryptBuilder] Regenerate rooms");
                    List<RotatedRectangle> rects = new();
                    for(int i = b.transform.childCount - 1; i >= 0; i--)
                        Undo.DestroyObjectImmediate(b.transform.GetChild(i).gameObject);
                    
                    foreach (var node in b.RectangleTree.Nodes)
                    {
                        var rectses = node.Rectangles;
                        if(rectses != null)
                        foreach (var r in rectses)
                        {
                            var rect = r;
                            CreateRectangleGameObject(ref rect, b);
                            rects.Add(rect);
                        }
                    }
                    b.RectangleTree = new();
                    foreach(var r in rects)
                    {
                        b.RectangleTree.AddRectangle(r);
                    }
                    EditorUtility.SetDirty(b);
                }
            }
            void OnSceneGUI()
            {
                var b = (Builder)target;

                Handles.matrix = Matrix4x4.TRS(b.transform.position, Quaternion.identity, Vector3.one);

                CryptHandles.RoundRectangleSize = b._rectRounding;
                CryptHandles.RoundRectangleRotation = b._rectRotationRounding;
                if (!(b.RectangleTree == null || b.RectangleTree.Count < 2))
                {
                    CryptHandles.DrawBoundingNode(1, b.RectangleTree, -1, b._debugShowBounds);
                }

                if(Event.current.type == EventType.KeyDown && 
                    Event.current.keyCode == KeyCode.V && 
                    Event.current.control && 
                    b._rectangleClipboard.Count > 0)
                {
                    Undo.RecordObject(b, "[CryptBuilder] Paste rectangle(s) from clipboard");
                    DeselectHeld(b);
                    b._editMode = EditMode.EditHeld;
                    foreach(var rect in b._rectangleClipboard)
                    {
                        var r = rect;
                        CreateRectangleGameObject(ref r, b);
                        r.Room.Style = rect.Room.Style;
                        b._heldRectangles.Add(r);
                    }
                    Event.current.Use();
                    EditorUtility.SetDirty(b);
                }

                switch (b._editMode)
                {
                    case EditMode.DontEdit:
                        DontEdit(b);
                        break;

                    case EditMode.AddNew:
                        AddNew(b);
                        break;

                    case EditMode.EditHeld:
                        EditHeld(b);
                        break;

                    case EditMode.StyleBrush:
                        StyleBrush(b);
                        break;
                }
            }

            void StyleBrush(Builder b)
            {
                if (Event.current.type == EventType.MouseMove)
                    SceneView.RepaintAll();

                bool useEvent = (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && Event.current.button == (int)MouseButton.Left;
                if(useEvent)
                {
                    Event.current.Use();
                }

                if (GetRectAtMouse(b, out int rect, out int node))
                {
                    Handles.color = GetStyleColor(b._brushStyle);
                    var hovered = b.RectangleTree.Nodes[node].Rectangles[rect];
                    CryptHandles.DrawRectangle(hovered);

                    if (hovered.Room.Style == b._brushStyle)
                        return;

                    if (useEvent)
                    {
                        Undo.RecordObject(b, "[CryptBuilder] Set style with brush");
                        Event.current.Use();
                        b.RectangleTree.Nodes[node].RemoveRectangle(rect, b.RectangleTree);
                        if (hovered.Room == null)
                            CreateRectangleGameObject(ref hovered, b);
                        hovered.Room.Style = b._brushStyle;
                        b.RectangleTree.AddRectangle(hovered);
                        EditorUtility.SetDirty(b);
                    }
                }
            }

            bool GetRectAtMouse(Builder b, out int rect, out int node)
            {
                rect = default;
                node = default;

                if (!TryTracePlaneFromMouse(b, out var mousePos)) return false;

                return b.RectangleTree.TryGetRectangleAtPoint(mousePos, out node, out rect);
            }

            void DontEdit(Builder b)
            {
                if(GetRectAtMouse(b, out int rect, out int node))
                {
                    Handles.color = Color.yellow;
                    var hovered = b.RectangleTree.Nodes[node].Rectangles[rect];
                    CryptHandles.DrawRectangle(hovered);
                    if (Event.current.type == EventType.MouseDown && Event.current.button == (int)MouseButton.Left)
                    {
                        Undo.RecordObject(b, "[CryptBuilder] Select rectangle");
                        Event.current.Use();
                        if (hovered.Room == null)
                            CreateRectangleGameObject(ref hovered, b);
                        b._heldRectangles.Add(hovered);
                        b._editMode = EditMode.EditHeld;
                        b.RectangleTree.Nodes[node].RemoveRectangle(rect, b.RectangleTree);
                        EditorUtility.SetDirty(b);
                    }
                }
                if (Event.current.type == EventType.MouseMove)
                    SceneView.RepaintAll();
            }

            Vector2 clickPosition;
            Vector2 dragPosition;
            bool draggingNew;
            void AddNew(Builder b)
            {
                switch (Event.current.type)
                {
                    case EventType.Repaint:
                        if (!draggingNew) 
                            return;

                        Handles.color = Color.white;
                        CryptHandles.DrawBoundingBox(new(clickPosition, dragPosition));
                        return;

                    case EventType.MouseDown:
                        if (Event.current.IsRightMouseButton()) 
                            return;

                        draggingNew = TryTracePlaneFromMouse(b, out clickPosition);
                        dragPosition = clickPosition;
                        if (draggingNew) 
                            Event.current.Use();
                        return;
                    
                    case EventType.MouseDrag:
                        if (!TryTracePlaneFromMouse(b, out dragPosition)) 
                            return;
                        
                        SceneView.RepaintAll();
                        Event.current.Use();
                        return;
                    
                    case EventType.MouseUp:
                        if (!draggingNew) 
                            return;

                        Undo.RecordObject(b, "[CryptBuilder] Add new rectangle");
                        var size = dragPosition - clickPosition;
                        size.x = Mathf.Abs(size.x);
                        size.y = Mathf.Abs(size.y);
                        var pos = (dragPosition + clickPosition) * .5f;
                        RotatedRectangle n = default;
                        n.HalfSize = .5f * size;
                        n.CenterPosition = pos;
                        CreateRectangleGameObject(ref n, b);
                        b._heldRectangles.Add(n);
                        b._editMode = EditMode.EditHeld;
                        draggingNew = false;
                        EditorUtility.SetDirty(b);
                        return;
                }
            }

            float lastUniformScale = 1;
            void EditHeld(Builder b)
            {
                if (b._heldRectangles.Count < 1)
                {
                    b._editMode = EditMode.DontEdit;
                    return;
                }

                foreach (var rect in b._heldRectangles)
                {
                    Handles.color = rect.IsValid ? Color.white : Color.red;
                    CryptHandles.DrawRectangle(rect);
                }

                if(!Event.current.shift)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector2 originalPos = b._heldRectangles[0].CenterPosition;
                    Vector3 pos = originalPos.To3D();
                    Vector3 scale = b._heldRectangles[0].HalfSize.To3D();
                    Quaternion rotation = Quaternion.AngleAxis(b._heldRectangles[0].Rotation, Vector3.up);

                    float uniformScale = lastUniformScale;
                    if (b._heldRectangles.Count > 1)
                        Handles.TransformHandle(ref pos, ref rotation, ref uniformScale);
                    else Handles.TransformHandle(ref pos, ref rotation, ref scale);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(b, "[CryptBuilder] Transform held rectangle(s)");
                        Vector2 posDif = pos.To2D();
                        Vector2 scaleDif = scale.To2D() / b._heldRectangles[0].HalfSize;
                        scaleDif *= uniformScale / lastUniformScale;
                        lastUniformScale = uniformScale;
                        float rotationDif = rotation.eulerAngles.y - b._heldRectangles[0].Rotation;

                        Matrix2x2 transform = Matrix2x2.FromRotationAngle(rotationDif);
                        transform.iHat *= scaleDif.x;
                        transform.jHat *= scaleDif.y;
                        
                        for(int i = 0; i<b._heldRectangles.Count; i++)
                        {
                            var rect = b._heldRectangles[i];
                            rect.CenterPosition -= originalPos;
                            rect.CenterPosition *= transform;
                            rect.CenterPosition += posDif;
                            rect.HalfSize *= scaleDif;
                            rect.Rotation += rotationDif;
                            b._heldRectangles[i] = rect;

                            var rounded = rect;
                            rounded.Round(b._rectRounding, b._rectRotationRounding);
                            rect.Room.transform.rotation = Quaternion.Euler(0, rounded.Rotation, 0);
                            rect.Room.transform.localPosition = rounded.CenterPosition.To3D();
                        }
                        EditorUtility.SetDirty(b);
                    }
                }

                if(Event.current.type == EventType.KeyDown)
                {
                    switch(Event.current.keyCode)
                    {
                        case KeyCode.Delete:
                        case KeyCode.Backspace:
                            Undo.RecordObject(b, "[CryptBuilder] Delete held rectangle(s)");
                            foreach(var rect in b._heldRectangles)
                                Undo.DestroyObjectImmediate(rect.Room.gameObject);
                            
                            b._heldRectangles.Clear();
                            b._editMode = EditMode.DontEdit;
                            Event.current.Use();
                            EditorUtility.SetDirty(b);
                            return;

                        case KeyCode.C:
                            if (Event.current.control)
                            {
                                Undo.RecordObject(b, "[CryptBuilder] Add held rectangle(s) to clipboard");
                                b._rectangleClipboard.Clear();
                                foreach (var rect in b._heldRectangles)
                                    b._rectangleClipboard.Add(rect);

                                EditorUtility.SetDirty(b);
                                Event.current.Use();
                            }
                            return;

                        case KeyCode.D:
                            if(Event.current.control)
                            {
                                Undo.RecordObject(b, "[CryptBuilder] Duplicate held rectangle(s)");
                                for(int i = 0; i < b._heldRectangles.Count; i++)
                                {
                                    var rect = b._heldRectangles [i];
                                    var r = rect;
                                    rect.Round(b._rectRounding, b._rectRotationRounding);
                                    b.RectangleTree.AddRectangle(rect);
                                    rect.CenterPosition += Vector2.one;
                                    CreateRectangleGameObject(ref rect, b);
                                    rect.Room.Style = r.Room.Style;
                                    b._heldRectangles [i] = rect;
                                }
                                EditorUtility.SetDirty(b);
                                Event.current.Use();
                            }
                            return;
                    }
                }

                if(Event.current.shift && GetRectAtMouse(b, out int rectIndex, out int node))
                {
                    Handles.color = Color.yellow;
                    var hovered = b.RectangleTree.Nodes[node].Rectangles[rectIndex];
                    CryptHandles.DrawRectangle(hovered);
                }
                if(Event.current.type == EventType.MouseDown && Event.current.button == (int)MouseButton.Left)
                {
                    if(Event.current.shift)
                    {
                        if (!TryTracePlaneFromMouse(b, out var mousePos))
                            return;
                        if(GetRectAtMouse(b, out rectIndex, out node))
                        {
                            Undo.RecordObject(b, "[CryptBuilder] Add rectangle to selection");
                            Event.current.Use();
                            b._heldRectangles.Add(b.RectangleTree.Nodes[node].Rectangles[rectIndex]);
                            b.RectangleTree.Nodes[node].RemoveRectangle(rectIndex, b.RectangleTree);
                            EditorUtility.SetDirty(b);
                            return;
                        }
                        for(int i = 0; i< b._heldRectangles.Count; i++)
                        {
                            var rect = b._heldRectangles[i];
                            if (rect.ContainsPoint(mousePos))
                            {
                                Undo.RecordObject(b, "[CryptBuilder] Deselect rectangle");
                                b._heldRectangles.RemoveAt(i);
                                if(b._heldRectangles.Count == 0)
                                    b._editMode = EditMode.DontEdit;
                                rect.Round(b._rectRounding, b._rectRotationRounding);
                                b.RectangleTree.AddRectangle(rect);
                                Event.current.Use();
                                EditorUtility.SetDirty(b);
                                return;
                            }
                        }
                    }
                    DeselectHeld(b);
                    Event.current.Use();
                }
            }

            void DeselectHeld(Builder b)
            {
                Undo.RecordObject(b, "[CryptBuilder] Deselect rectangle(s)");
                b._editMode = EditMode.DontEdit;
                foreach (var rect in b._heldRectangles)
                {
                    rect.Round(b._rectRounding, b._rectRotationRounding);
                    b.RectangleTree.AddRectangle(rect);
                }
                b._heldRectangles.Clear();
                EditorUtility.SetDirty(b);
            }

            static bool TryTracePlaneFromMouse(Builder b, out Vector2 position)
            {
                var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                position = default;
                ray.origin -= b.transform.position;
                float t = ray.origin.y / -ray.direction.y;
                if (t < 0 || float.IsNaN(t))
                    return false;

                position = (ray.origin + ray.direction * t).To2D();
                return true;
            }

        }
        enum EditMode
        {
            DontEdit = default,
            AddNew,
            EditHeld,
            StyleBrush
        }
        struct HitboxGenerator : ICryptSurfaceGenerator
        {
            public float TileScale;
            public CryptRoomStyle DefaultStyle;
            RotatedRectangle _room;
            Vector2 _tileOffset;
            bool _validRoom;
            float _roomHeight;

            public void GenerateFloor(BoundingBox shape)
            {
                if (!_validRoom) return;

                var col = _room.Room.Colliders.AddComponent<BoxCollider>();
                var scale = shape.Size;

                col.size = new(scale.x, 1f, scale.y);
                col.center = new(0, -.5f, 0);
                
                col = _room.Room.Colliders.AddComponent<BoxCollider>();

                col.size = new(scale.x, 1f, scale.y);
                col.center = new(0, .5f + _roomHeight, 0);
            }

            public void GenerateWall(Vector2 start, Vector2 end, Vector2 normal)
            {
                if (!_validRoom) return;

                start -= _tileOffset;
                end -= _tileOffset;

                var col = _room.Room.Colliders.AddComponent<BoxCollider>();
                float length = Vector2.Distance(start, end);

                Vector2 direction = (start - end);
                normal *= TileScale;
                col.size = new(Mathf.Abs(direction.x) + Mathf.Abs(normal.x), 4, Mathf.Abs(direction.y) + Mathf.Abs(normal.y));
                Vector2 center = .5f * (start + end + normal);
                col.center = new(center.x, 1, center.y);
            }

            public void OnNewRoom(RotatedRectangle room)
            {
                _validRoom = room.Room != null;
                if (!_validRoom)
                    Debug.LogError("Room is missing a generated room object! Click \"Regenerate rooms\" on the cryptbuilder.");
                else
                {
                    Undo.DestroyObjectImmediate(room.Room.Colliders);
                    _tileOffset = room.CenterPosition;
                    room.Room.Colliders.transform.rotation = Quaternion.identity;
                }

                _room = room;
                _roomHeight = 2;
                var style = room.Room.Style;
                if(style == null ) style = DefaultStyle;
                if(style != null && style.WallHeight > 0)
                    _roomHeight = style.WallHeight;
            }
        }
    }
}
#endif