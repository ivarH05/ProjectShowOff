using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace CryptBuilder
{
#if UNITY_EDITOR
    public partial class Builder
    {
        [SerializeField, HideInInspector] EditMode _editMode;
        [SerializeField, HideInInspector] List<RotatedRectangle> _heldRectangles = new();
        [SerializeField, HideInInspector] GameObject _generatedCrypt;
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
                    var newStyle = EditorGUILayout.ObjectField(rectCurrent.Style, typeof(CryptRoomStyle), false);
                    EditorGUILayout.EndHorizontal();

                    if (newStyle != rectCurrent.Style)
                    {
                        Undo.RecordObject(b, "[CryptBuilder] Set selected room's style");
                        rectCurrent.Style = (CryptRoomStyle)newStyle;
                        b._heldRectangles[0] = rectCurrent;
                        EditorUtility.SetDirty(b);
                    }
                }
                
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
                if (GUILayout.Button("Reset crypt"))
                {
                    Undo.RecordObject(b, "[CryptBuilder] Reset crypt");
                    b.RectangleTree = new();
                    SceneView.RepaintAll();
                    EditorUtility.SetDirty(b);
                }
                if(b._generatedCrypt != null)
                {
                    if(GUILayout.Button("Delete generated crypt"))
                    {
                        Undo.DestroyObjectImmediate(b._generatedCrypt);
                        b._generatedCrypt = null;
                        EditorUtility.SetDirty(b);
                    }
                }
                else if(GUILayout.Button("Generate full crypt"))
                {
                    b._generatedCrypt = new("Crypt");
                    Undo.RegisterCreatedObjectUndo(b._generatedCrypt, "[CryptBuilder] Generate crypt");
                    b._generatedCrypt.transform.localPosition = b.transform.position;
                    CryptGenerator gen = new();
                    gen.DefaultStyle = b._defaultStyle;
                    gen.CryptRoot = b._generatedCrypt;
                    b.GenerateCrypt(gen);
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
                        b._heldRectangles.Add(rect);
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

                    if (hovered.Style == b._brushStyle)
                        return;

                    if (useEvent)
                    {
                        Undo.RecordObject(b, "[CryptBuilder] Set style with brush");
                        Event.current.Use();
                        b.RectangleTree.Nodes[node].RemoveRectangle(rect, b.RectangleTree);
                        hovered.Style = b._brushStyle;
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
                                    rect.Round(b._rectRounding, b._rectRotationRounding);
                                    b.RectangleTree.AddRectangle(rect);
                                    rect.CenterPosition += Vector2.one;
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
    }
#endif
}