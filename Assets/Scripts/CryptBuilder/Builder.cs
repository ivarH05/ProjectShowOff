using System.Collections.Generic;
using UnityEngine;

namespace CryptBuilder
{
    public partial class Builder : MonoBehaviour
    {
        [field:SerializeField] public RectangleCollection RectangleTree {get; private set; }
        [SerializeField] float _rectRounding = .25f;
        [SerializeField] float _rectRotationRounding = 90;

        private void OnDrawGizmosSelected()
        {
            List<LineSegment> shape = new();

            GenerateShapeRecursive(1);

            void GenerateShapeRecursive(int nodeIndex)
            {
                var node = RectangleTree.Nodes[nodeIndex];
                var rects = node.Rectangles;
                if(rects != null)
                {
                    for(int i = 0; i<rects.Count; i++)
                    {
                        GenerateShape(shape, nodeIndex, i);
                        Gizmos.color = Color.white;
                        int index = i*10;
                        foreach(var l in shape)
                        {
                            index++;
                            Gizmos.DrawLine(l.A.To3D(index * .05f), l.B.To3D(index * .05f));
                        }
                    }
                }
                if(node.ChildAIndex > 0)
                {
                    GenerateShapeRecursive(node.ChildAIndex);
                    GenerateShapeRecursive(node.ChildBIndex);
                }
            }
        }

        void GenerateShape(List<LineSegment> shape, int nodeIndex, int rectangleIndex)
        {
            shape.Clear();
            var rootNode = RectangleTree.Nodes[nodeIndex];
            var rects = rootNode.Rectangles;
            var shapeRect = rects[rectangleIndex];
            foreach (var l in shapeRect.GetLines())
                shape.Add(l);

            var colCheck = shapeRect.GetBounds();
            for(int i = 0; i<rects.Count; i++)
            {
                if (i == rectangleIndex) continue;
                var rect = rects[i];
                if (!rect.GetBounds().Intersects(colCheck)) continue;
                CutRectOutOfShape(rect, shape);
            }
        }

        void CutRectOutOfShape(RotatedRectangle rect, List<LineSegment> shape)
        {
            foreach (LineSegment line in rect.GetLines())
            {
                int shapeCount = shape.Count;
                for(int i = 0; i<shapeCount; i++)
                {
                    LineSegment shapeLine = shape[i];
                    if (shapeLine.TIntersection(line, out float t))
                    {
                        var originalA = shapeLine.A;
                        shapeLine.A = Vector2.LerpUnclamped(shapeLine.A, shapeLine.B, t);
                        
                        shape[i] = shapeLine;
                        shape.Add(new(originalA, shapeLine.A));
                    }
                    else if(float.IsNaN(t))
                    {
                        // check overlapping lines here
                    }
                }
            }
            for (int i = shape.Count - 1; i >= 0; i--)
            {
                Vector2 offset = shape[i].Normal * .5f * _rectRounding;
                if (rect.ContainsPoint(shape[i].A + offset) && 
                    rect.ContainsPoint(shape[i].B + offset))
                    shape.RemoveAt(i);
            }
        }
    }
}
