using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace CryptBuilder
{
    public partial class Builder
    {
        [CustomEditor(typeof(Builder))]
        class BuilderEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                var b = (Builder)target;
                if(GUILayout.Button("Reset crypt"))
                {
                    b.RectangleTree = new();
                    SceneView.RepaintAll();
                }
                if(GUILayout.Button("Add random rect"))
                {
                    RotatedRectangle rect = new();
                    rect.CenterPosition = new(Random.Range(0,66), Random.Range(0,66));
                    rect.HalfSize = new(Random.Range(1f,3),Random.Range(1f,3));
                    rect.Rotation = Random.Range(0, 360);
                    b.RectangleTree.AddRectangle(rect);
                    SceneView.RepaintAll();
                }
            }
            void OnSceneGUI()
            {
                var b = (Builder)target;
                Handles.color = b._heldRectangle.IsValid ? Color.white : Color.red;
                DrawRectangle(b._heldRectangle);
                var bb = b._heldRectangle.GetBounds();
                Handles.color = Color.white;
                DrawBoundingBox(bb);

                if(b.RectangleTree != null && b.RectangleTree.Count > 1)
                    DrawBoundingNode(1, b.RectangleTree, 0);

                EditorGUI.BeginChangeCheck();
                Vector3 pos = b._heldRectangle.CenterPosition.To3D();
                Vector3 scale = b._heldRectangle.HalfSize.To3D();
                Quaternion rotation = Quaternion.AngleAxis(b._heldRectangle.Rotation, Vector3.up);
                Handles.TransformHandle(ref pos, ref rotation, ref scale);
                if(EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(b, "Change held rectangle transform");
                    b._heldRectangle.CenterPosition = pos.To2D();
                    b._heldRectangle.HalfSize = scale.To2D();
                    b._heldRectangle.Rotation = rotation.eulerAngles.y;
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
                var col = Color.Lerp(Color.lawnGreen, Color.aquamarine, 1f / (.4f * depth + 1));
                col.a = .4f;
                Handles.color = col;
                var node = owner.Nodes[nodeIndex];
                DrawBoundingBox(node.Bounds);
                if(node.Rectangles != null)
                {
                    col.a = 1f;
                    Handles.color = col;
                    foreach (var r in node.Rectangles)
                    {
                        DrawRectangle(r);
                    }
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

            enum EditMode
            {
                DontEdit,
                AddNew,
                EditHeld
            }
        }
    }
}