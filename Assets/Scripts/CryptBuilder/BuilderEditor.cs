using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;

namespace CryptBuilder
{
    public partial class Builder
    {
        [SerializeField] bool _holdingRectangle;
        [SerializeField] EditMode _editMode;
        [SerializeField] RotatedRectangle _heldRectangle;

        [CustomEditor(typeof(Builder))]
        class BuilderEditor : Editor
        {
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

            void DontEdit(Builder b)
            {
                if (TryTracePlaneFromMouse(out var mousePos))
                {
                    if (b.RectangleTree.Nodes[1].TryGetRectangleAtPoint(mousePos, b.RectangleTree, out int rect, out int node))
                    {
                        Handles.color = Color.yellow;
                        var hovered = b.RectangleTree.Nodes[node].Rectangles[rect];
                        DrawRectangle(hovered);
                        if (Event.current.type == EventType.MouseDown && Event.current.button == (int)MouseButton.Left)
                        {
                            Undo.RecordObject(b, "Select rectangle");
                            Event.current.Use();
                            b._heldRectangle = hovered;
                            b._holdingRectangle = true;
                            b._editMode = EditMode.EditHeld;
                            b.RectangleTree.Nodes[node].RemoveRectangle(rect, b.RectangleTree);
                            EditorUtility.SetDirty(b);
                        }
                    }
                    if (Event.current.type == EventType.MouseMove)
                        SceneView.RepaintAll();
                }
            }

            void EditHeld(Builder b)
            {
                if (!b._holdingRectangle)
                {
                    b._editMode = EditMode.DontEdit;
                    return;
                }

                Handles.color = b._heldRectangle.IsValid ? Color.white : Color.red;
                DrawRectangle(b._heldRectangle);

                EditorGUI.BeginChangeCheck();
                Vector3 pos = b._heldRectangle.CenterPosition.To3D();
                Vector3 scale = b._heldRectangle.HalfSize.To3D();
                Quaternion rotation = Quaternion.AngleAxis(b._heldRectangle.Rotation, Vector3.up);
                Handles.TransformHandle(ref pos, ref rotation, ref scale);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(b, "Transform held rectangle");
                    b._heldRectangle.CenterPosition = pos.To2D();
                    b._heldRectangle.HalfSize = scale.To2D();
                    b._heldRectangle.Rotation = rotation.eulerAngles.y;
                    EditorUtility.SetDirty(b);
                }

                if(Event.current.type == EventType.KeyDown)
                {
                    switch(Event.current.keyCode)
                    {
                        case KeyCode.Delete:
                        case KeyCode.Backspace:
                            Undo.RecordObject(b, "Delete held rectangle");
                            b._holdingRectangle = false;
                            b._editMode = EditMode.DontEdit;
                            Event.current.Use();
                            EditorUtility.SetDirty(b);
                            return;

                        case KeyCode.C:
                            if (Event.current.control)
                            {
                                Undo.RecordObject(b, "Duplicate held rectangle");
                                b.RectangleTree.AddRectangle(b._heldRectangle);
                                EditorUtility.SetDirty(b);
                            }
                            return;
                    }
                }

                if(Event.current.type == EventType.MouseDown && Event.current.button == (int)MouseButton.Left)
                {
                    Undo.RecordObject(b, "Deselect rectangle");
                    b._holdingRectangle = false;
                    b._editMode = EditMode.DontEdit;
                    b.RectangleTree.AddRectangle(b._heldRectangle);
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