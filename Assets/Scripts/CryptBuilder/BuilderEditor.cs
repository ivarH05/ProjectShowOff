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
                DrawRectangle(b.TestRectangle);
                var bb = b.TestRectangle.GetBounds();
                DrawBoundingBox(bb);
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
        }
    }
}