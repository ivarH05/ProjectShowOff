using System.Collections.Generic;
using UnityEngine;

namespace CryptBuilder
{
    public partial class Builder : MonoBehaviour
    {
        [field:SerializeField, HideInInspector] public RectangleCollection RectangleTree {get; private set; }
        [SerializeField] float _rectRounding = .25f;
        float _rectRotationRounding = 90;

        private void OnDrawGizmosSelected()
        {
            GenerateShapeRecursive(1);

            void GenerateShapeRecursive(int nodeIndex)
            {
                var node = RectangleTree.Nodes[nodeIndex];
                var rects = node.Rectangles;
                if(rects != null)
                {
                    for(int i = 0; i<rects.Count; i++)
                    {
                        GenerateTiles(nodeIndex, i);
                    }
                }
                if(node.ChildAIndex > 0)
                {
                    GenerateShapeRecursive(node.ChildAIndex);
                    GenerateShapeRecursive(node.ChildBIndex);
                }
            }
        }

        void GenerateTiles(int nodeIndex, int rectIndex)
        {
            var node = RectangleTree.Nodes[nodeIndex];
            var rects = node.Rectangles;
            var rect = rects[rectIndex];
            var bounds = rect.GetBounds();
            for (float x = bounds.Minimum.x + .5f*_rectRounding; x < bounds.Maximum.x; x += _rectRounding)
            {
                for(float y = bounds.Minimum.y + .5f * _rectRounding; y < bounds.Maximum.y; y += _rectRounding)
                {
                    Vector2 point = new(x, y);
                    if (!(RectangleTree.TryGetRectangleAtPoint(point, out int nodeI, out int rectI) && nodeI == nodeIndex && rectI == rectIndex))
                        continue;

                    GenerateFloor(point);

                    Vector2 testEdge;
                    Vector2 normal = new(1, 0);
                    testEdge = point + normal * _rectRounding;
                    if (testEdge.x > bounds.Maximum.x && !RectangleTree.TryGetRectangleAtPoint(testEdge, out _, out _))
                        GenerateWall(testEdge, normal);

                    normal = new(-1, 0);
                    testEdge = point + normal * _rectRounding;
                    if (testEdge.x < bounds.Minimum.x && !RectangleTree.TryGetRectangleAtPoint(testEdge, out _, out _))
                        GenerateWall(testEdge, normal);

                    normal = new(0, 1);
                    testEdge = point + normal * _rectRounding;
                    if (testEdge.y > bounds.Maximum.y && !RectangleTree.TryGetRectangleAtPoint(testEdge, out _, out _))
                        GenerateWall(testEdge, normal);

                    normal = new(0, -1);
                    testEdge = point + normal * _rectRounding;
                    if (testEdge.y < bounds.Minimum.y && !RectangleTree.TryGetRectangleAtPoint(testEdge, out _, out _))
                        GenerateWall(testEdge, normal);
                }
            }
        }

        void GenerateFloor(Vector2 point)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(point.To3D(), point.To3D(.1f));
        }

        void GenerateWall(Vector2 point, Vector2 normal)
        {
            Gizmos.color = Color.orange;
            Gizmos.DrawLine(point.To3D(), (point + normal * _rectRounding * .5f).To3D());
        }
    }
}
