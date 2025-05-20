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
                Vector2 otherCorner1 = new(bb.Minimum.x, bb.Maximum.y);
                Vector2 otherCorner2 = new(bb.Maximum.x, bb.Minimum.y);
                Handles.DrawLine(bb.Minimum.To3D(), bb.Maximum.To3D());
                Handles.DrawLine(otherCorner1.To3D(), otherCorner2.To3D());
            }

            static void DrawRectangle(RotatedRectangle rect)
            {
                foreach (var line in rect.GetLines())
                {
                    Handles.DrawLine(line.A.To3D(), line.B.To3D());
                }
            }
        }
    }
}