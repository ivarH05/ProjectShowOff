using UnityEditor;
using UnityEngine;

namespace CryptBuilder
{
    public static class CryptHandles
    {
        public static float RoundRectangleSize = .25f;
        public static float RoundRectangleRotation = 90f;


        public static void DrawBoundingNode(int nodeIndex, RectangleCollection owner, int depth, bool showBounds)
        {
            var col = Color.Lerp(Color.deepPink, Color.lawnGreen, Mathf.Exp(-depth * .2f));
            col.a = .4f;
            Handles.color = col;
            var node = owner.Nodes[nodeIndex];
            if (showBounds)
                DrawBoundingBox(node.Bounds, .1f);
            if (node.Rectangles != null)
            {
                col.a = 1f;
                Handles.color = col;
                foreach (var r in node.Rectangles)
                    DrawRectangle(r);
            }
            if (node.ChildAIndex > 0)
            {
                DrawBoundingNode(node.ChildAIndex, owner, depth + 1, showBounds);
                DrawBoundingNode(node.ChildBIndex, owner, depth + 1, showBounds);
            }
        }
        public static void DrawRectangle(RotatedRectangle rect)
        {
            rect.Round(RoundRectangleSize, RoundRectangleRotation);
            foreach (var line in rect.GetLines())
            {
                Handles.DrawLine(line.A.To3D(), line.B.To3D());
                //Vector2 avgPos = line.A + line.B;
                //avgPos *= .5f;
                //Handles.DrawLine(avgPos.To3D(), (avgPos + line.Normal).To3D());
            }
        }
        public static void DrawBoundingBox(BoundingBox box, float height = 0)
        {
            var ogcol = Handles.color;
            if (!box.IsValid) Handles.color *= Color.pink;
            Vector2 otherCorner1 = new(box.Minimum.x, box.Maximum.y);
            Vector2 otherCorner2 = new(box.Maximum.x, box.Minimum.y);
            Handles.DrawLine(otherCorner1.To3D(height), box.Minimum.To3D(height));
            Handles.DrawLine(otherCorner1.To3D(height), box.Maximum.To3D(height));
            Handles.DrawLine(otherCorner2.To3D(height), box.Maximum.To3D(height));
            Handles.DrawLine(otherCorner2.To3D(height), box.Minimum.To3D(height));
            Handles.color = ogcol;
        }
    }
}

