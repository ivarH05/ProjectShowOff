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

        [CustomEditor(typeof(Builder))]
        class BuilderEditor : Editor
        {
            private void OnDisable()
            {
                var b = (Builder)target;
                if(b._heldRectangles.Count > 0)
                {
                    Undo.RecordObject(b, "Deselect rectangle");
                    b._editMode = EditMode.DontEdit;
                    foreach(RotatedRectangle rect in b._heldRectangles)
                        b.RectangleTree.AddRectangle(rect);
                    EditorUtility.SetDirty(b);
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
                if (GUILayout.Button("Add random rect"))
                {
                    Undo.RecordObject(b, "Add random rectangle");
                    RotatedRectangle rect = new();
                    rect.CenterPosition = new(Random.Range(0,66), Random.Range(0,66));
                    rect.HalfSize = new(Random.Range(1f,3),Random.Range(1f,3));
                    rect.Rotation = Random.Range(0, 360);
                    b.RectangleTree.AddRectangle(rect);
                    SceneView.RepaintAll();
                    EditorUtility.SetDirty(b);
                }
                if(GUILayout.Button("Add new rect"))
                {
                    b._editMode = EditMode.AddNew;
                }
            }
            void OnSceneGUI()
            {
                var b = (Builder)target;

                if (b.RectangleTree == null || b.RectangleTree.Count < 2)
                    return;
                
                DrawBoundingNode(1, b.RectangleTree, 0);
                switch(b._editMode)
                {
                    case EditMode.DontEdit:
                        DontEdit(b);
                        break;

                    case EditMode.AddNew:
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
                    Vector3 pos = b._heldRectangles[0].CenterPosition.To3D();
                    Vector3 scale = b._heldRectangles[0].HalfSize.To3D();
                    Quaternion rotation = Quaternion.AngleAxis(b._heldRectangles[0].Rotation, Vector3.up);
                    Handles.TransformHandle(ref pos, ref rotation, ref scale);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(b, "Transform held rectangle(s)");
                        Vector2 posDif = pos.To2D() - b._heldRectangles[0].CenterPosition;
                        Vector2 scaleDif = scale.To2D() / b._heldRectangles[0].HalfSize;
                        float rotationDif = rotation.eulerAngles.y - b._heldRectangles[0].Rotation;
                        for(int i = 0; i<b._heldRectangles.Count; i++)
                        {
                            var rect = b._heldRectangles[i];
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
                            Undo.RecordObject(b, "Delete held rectangle");
                            b._heldRectangles.Clear();
                            b._editMode = EditMode.DontEdit;
                            Event.current.Use();
                            EditorUtility.SetDirty(b);
                            return;

                        case KeyCode.C:
                            if (Event.current.control)
                            {
                                Undo.RecordObject(b, "Duplicate held rectangle");
                                foreach(var rect in b._heldRectangles)
                                    b.RectangleTree.AddRectangle(rect);
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
                    Undo.RecordObject(b, "Deselect rectangle(s)");
                    b._editMode = EditMode.DontEdit;
                    foreach(var rect in b._heldRectangles)
                        b.RectangleTree.AddRectangle(rect);
                    b._heldRectangles.Clear();
                    Event.current.Use();
                    EditorUtility.SetDirty(b);
                }
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
            static void DrawBoundingNode(int nodeIndex, RectangleCollection owner, int depth)
            {
                var col = Color.Lerp(Color.lawnGreen, Color.deepPink, Mathf.Exp(-depth * .2f));
                col.a = .4f;
                Handles.color = col;
                var node = owner.Nodes[nodeIndex];
                //DrawBoundingBox(node.Bounds);
                if(node.Rectangles != null)
                {
                    col.a = 1f;
                    Handles.color = col;
                    foreach (var r in node.Rectangles)
                        DrawRectangle(r);
                }
                if(node.ChildAIndex > 0)
                {
                    DrawBoundingNode(node.ChildAIndex, owner, depth+1);
                    DrawBoundingNode(node.ChildBIndex, owner, depth+1);
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