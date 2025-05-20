using UnityEditor;
using UnityEngine;

namespace CryptBuilder
{
    public partial class Builder
    {
        [CustomEditor(typeof(Builder))]
        class BuilderEditor : Editor
        {
            void OnSceneGUI()
            {
                var b = (Builder)target;
                Handles.color = b._heldRectangle.IsValid ? Color.white : Color.red;
                DrawRectangle(b._heldRectangle);
                var bb = b._heldRectangle.GetBounds();
                Handles.color = Color.white;
                DrawBoundingBox(bb);

                Vector3 pos = b._heldRectangle.CenterPosition.To3D();
                Vector3 scale = b._heldRectangle.HalfSize.To3D();
                Quaternion rotation = Quaternion.AngleAxis(b._heldRectangle.Rotation, Vector3.up);
                Handles.TransformHandle(ref pos, ref rotation, ref scale);
                b._heldRectangle.CenterPosition = pos.To2D();
                b._heldRectangle.HalfSize = scale.To2D();
                b._heldRectangle.Rotation = rotation.eulerAngles.y;
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
                Vector2 otherCorner1 = new(box.Minimum.x, box.Maximum.y);
                Vector2 otherCorner2 = new(box.Maximum.x, box.Minimum.y);
                Handles.DrawLine(otherCorner1.To3D(), box.Minimum.To3D());
                Handles.DrawLine(otherCorner1.To3D(), box.Maximum.To3D());
                Handles.DrawLine(otherCorner2.To3D(), box.Maximum.To3D());
                Handles.DrawLine(otherCorner2.To3D(), box.Minimum.To3D());
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