using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace CryptBuilder
{
    public partial class Builder
    {
        [SerializeField] EditMode _editMode;
        [SerializeField] List<RotatedRectangle> _heldRectangles = new();
        bool _debugShowBounds;

        [CustomEditor(typeof(Builder))]
        class BuilderEditor : Editor
        {
            private void OnDisable()
            {
                var b = (Builder)target;
                if(b._heldRectangles.Count > 0)
                {
                    DeselectHeld(b);
                }
            }
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                var b = (Builder)target;
                if(GUILayout.Button("Reset crypt"))
                {
                    Undo.RecordObject(b, "Reset crypt");
                    b.RectangleTree = new();
                    SceneView.RepaintAll();
                    EditorUtility.SetDirty(b);
                }
                if(GUILayout.Button("Add new rect"))
                {
                    DeselectHeld(b);
                    b._editMode = EditMode.AddNew;
                }
                if(GUILayout.Button("Toggle debug bounds"))
                {
                    b._debugShowBounds = !b._debugShowBounds;
                    SceneView.RepaintAll() ;
                }
                if(GUILayout.Button("Regenerate tree (may improve performance)"))
                {
                    b.RectangleTree.Regenerate();
                    SceneView.RepaintAll();
                }

            }
            void OnSceneGUI()
            {
                var b = (Builder)target;

                if (b.RectangleTree == null || b.RectangleTree.Count < 2)
                    return;
                
                DrawBoundingNode(1, b.RectangleTree, 0, b);
                switch(b._editMode)
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
                }
            }

            bool GetRectAtMouse(Builder b, out int rect, out int node)
            {
                rect = default;
                node = default;

                if (!TryTracePlaneFromMouse(out var mousePos)) return false;

                return b.RectangleTree.Nodes[1].TryGetRectangleAtPoint(mousePos, b.RectangleTree, out rect, out node);
            }

            void DontEdit(Builder b)
            {
                if(GetRectAtMouse(b, out int rect, out int node))
                {
                    Handles.color = Color.yellow;
                    var hovered = b.RectangleTree.Nodes[node].Rectangles[rect];
                    DrawRectangle(hovered);
                    if (Event.current.type == EventType.MouseDown && Event.current.button == (int)MouseButton.Left)
                    {
                        Undo.RecordObject(b, "Select rectangle");
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
                        DrawBoundingBox(new(clickPosition, dragPosition));
                        return;

                    case EventType.MouseDown:
                        if (Event.current.IsRightMouseButton()) 
                            return;

                        draggingNew = TryTracePlaneFromMouse(out clickPosition);
                        dragPosition = clickPosition;
                        if (draggingNew) 
                            Event.current.Use();
                        return;
                    
                    case EventType.MouseDrag:
                        if (!TryTracePlaneFromMouse(out dragPosition)) 
                            return;
                        
                        SceneView.RepaintAll();
                        Event.current.Use();
                        return;
                    
                    case EventType.MouseUp:
                        if (!draggingNew) 
                            return;

                        Undo.RecordObject(b, "Add new rectangle");
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
                    DrawRectangle(rect);
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
                        Undo.RecordObject(b, "Transform held rectangle(s)");
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
                            Undo.RecordObject(b, "Delete held rectangle(s)");
                            b._heldRectangles.Clear();
                            b._editMode = EditMode.DontEdit;
                            Event.current.Use();
                            EditorUtility.SetDirty(b);
                            return;

                        case KeyCode.C:
                            if (Event.current.control)
                            {
                                Undo.RecordObject(b, "Duplicate held rectangle(s)");
                                for(int i = 0; i < b._heldRectangles.Count; i++)
                                {
                                    var rect = b._heldRectangles [i];
                                    b.RectangleTree.AddRectangle(rect);
                                    rect.CenterPosition += Vector2.one;
                                    b._heldRectangles [i] = rect;
                                }
                                EditorUtility.SetDirty(b);
                            }
                            return;
                    }
                }

                if(Event.current.shift && GetRectAtMouse(b, out int rectIndex, out int node))
                {
                    Handles.color = Color.yellow;
                    var hovered = b.RectangleTree.Nodes[node].Rectangles[rectIndex];
                    DrawRectangle(hovered);
                }
                if(Event.current.type == EventType.MouseDown && Event.current.button == (int)MouseButton.Left)
                {
                    if(Event.current.shift)
                    {
                        if (!TryTracePlaneFromMouse(out var mousePos))
                            return;
                        if(GetRectAtMouse(b, out rectIndex, out node))
                        {
                            Undo.RecordObject(b, "Add rectangle to selection");
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
                                Undo.RecordObject(b, "Deselect rectangle");
                                b._heldRectangles.RemoveAt(i);
                                if(b._heldRectangles.Count == 0)
                                    b._editMode = EditMode.DontEdit;
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
                Undo.RecordObject(b, "Deselect rectangle(s)");
                b._editMode = EditMode.DontEdit;
                foreach (var rect in b._heldRectangles)
                    b.RectangleTree.AddRectangle(rect);
                b._heldRectangles.Clear();
                EditorUtility.SetDirty(b);
            }

            static bool TryTracePlaneFromMouse(out Vector2 position)
            {
                var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                position = default;
                float t = ray.origin.y / -ray.direction.y;
                if (t < 0 || float.IsNaN(t))
                    return false;

                position = (ray.origin + ray.direction * t).To2D();
                return true;
            }
            static void DrawBoundingNode(int nodeIndex, RectangleCollection owner, int depth, Builder b)
            {
                var col = Color.Lerp(Color.lawnGreen, Color.deepPink, Mathf.Exp(-depth * .2f));
                col.a = .4f;
                Handles.color = col;
                var node = owner.Nodes[nodeIndex];
                if(b._debugShowBounds) 
                    DrawBoundingBox(node.Bounds);
                if(node.Rectangles != null)
                {
                    col.a = 1f;
                    Handles.color = col;
                    foreach (var r in node.Rectangles)
                        DrawRectangle(r);
                }
                if(node.ChildAIndex > 0)
                {
                    DrawBoundingNode(node.ChildAIndex, owner, depth+1, b);
                    DrawBoundingNode(node.ChildBIndex, owner, depth+1, b);
                }
            }
            static void DrawRectangle(RotatedRectangle rect)
            {
                foreach (var line in rect.GetLines())
                {
                    Handles.DrawLine(line.A.To3D(), line.B.To3D());
                }
            }
            static void DrawBoundingBox(BoundingBox box)
            {
                var ogcol = Handles.color;
                if(!box.IsValid) Handles.color *= Color.pink;
                Vector2 otherCorner1 = new(box.Minimum.x, box.Maximum.y);
                Vector2 otherCorner2 = new(box.Maximum.x, box.Minimum.y);
                Handles.DrawLine(otherCorner1.To3D(), box.Minimum.To3D());
                Handles.DrawLine(otherCorner1.To3D(), box.Maximum.To3D());
                Handles.DrawLine(otherCorner2.To3D(), box.Maximum.To3D());
                Handles.DrawLine(otherCorner2.To3D(), box.Minimum.To3D());
                Handles.color = ogcol;
            }

        }
        enum EditMode
        {
            DontEdit = default,
            AddNew,
            EditHeld
        }
    }
}