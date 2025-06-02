using System.Collections.Generic;
using UnityEngine;

namespace CryptBuilder
{
    public partial class Builder : MonoBehaviour
    {
        [field:SerializeField] public RectangleCollection RectangleTree {get; private set; }
        [SerializeField] float _rectRounding = .25f;
        [SerializeField] CryptRoomStyle _defaultStyle;

        float _rectRotationRounding = 90;

        private void OnDrawGizmosSelected()
        {
            if (_rectRounding < 0.01f) 
                return;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
            GizmoGenerator gen = new();
            gen.RectRounding = _rectRounding;
            GenerateCrypt(gen);
        }

        /// <summary>
        /// Generates the crypt using the given generator.
        /// </summary>
        public void GenerateCrypt<TGenerator>(TGenerator generator) where TGenerator : ICryptGenerator
        {
            GenerateShapeRecursive(1, ref generator);

            void GenerateShapeRecursive(int nodeIndex, ref TGenerator gen)
            {
                var node = RectangleTree.Nodes[nodeIndex];
                var rects = node.Rectangles;
                if (rects != null)
                {
                    for (int i = 0; i < rects.Count; i++)
                    {
                        GenerateTiles(nodeIndex, i, ref gen);
                    }
                }
                if (node.ChildAIndex > 0)
                {
                    GenerateShapeRecursive(node.ChildAIndex, ref gen);
                    GenerateShapeRecursive(node.ChildBIndex, ref gen);
                }
            }
        }

        void GenerateTiles<TGenerator>(int nodeIndex, int rectIndex, ref TGenerator gen) where TGenerator : ICryptGenerator
        {
            var node = RectangleTree.Nodes[nodeIndex];
            var rects = node.Rectangles;
            var rect = rects[rectIndex];
            var bounds = rect.GetBounds();
            float thisPriority = bounds.Size.x * bounds.Size.y;

            var contestingRects = RectangleTree.GetRectanglesIntersectingBox(bounds);
            List<BoundingBox> higherPriorityRects = new();
            foreach ((int cNode, int cRect) in contestingRects)
            {
                if (cNode == nodeIndex && cRect == rectIndex) 
                    continue;

                var contestRect = RectangleTree.Nodes[cNode].Rectangles[cRect].GetBounds();
                var contestPriority = contestRect.Size.x * contestRect.Size.y;

                if (contestPriority < thisPriority)
                    continue;

                if (contestPriority > thisPriority)
                {
                    higherPriorityRects.Add(contestRect);
                    continue;
                }

                var contestCenter = contestRect.Center;
                var thisCenter = bounds.Center;
                if (contestCenter.y > thisCenter.y ||
                    contestCenter.x > thisCenter.x ||
                    contestRect.GetHashCode() > bounds.GetHashCode())
                {
                    higherPriorityRects.Add(contestRect);
                    continue;
                }

                // if there are two literally equal rectangles, idk if i can do anything about it
                // the room will just generate double like whatever
            }

            gen.OnNewRoom(rect);

            for (float x = bounds.Minimum.x + .5f*_rectRounding; x < bounds.Maximum.x; x += _rectRounding)
            {
                for(float y = bounds.Minimum.y + .5f * _rectRounding; y < bounds.Maximum.y; y += _rectRounding)
                {
                    Vector2 point = new(x, y);
                    bool lowerPriority = false;
                    foreach (var r in higherPriorityRects)
                        if (r.ContainsPoint(point))
                        {
                            lowerPriority = true;
                            break;
                        }
                    if (lowerPriority) continue;

                    gen.GenerateFloor(point);

                    Vector2 testEdge;
                    Vector2 normal = new(1, 0);
                    testEdge = point + normal * _rectRounding;
                    if (testEdge.x > bounds.Maximum.x && !RectangleTree.TryGetRectangleAtPoint(testEdge, out _, out _))
                        gen.GenerateWall(testEdge, normal);

                    normal = new(-1, 0);
                    testEdge = point + normal * _rectRounding;
                    if (testEdge.x < bounds.Minimum.x && !RectangleTree.TryGetRectangleAtPoint(testEdge, out _, out _))
                        gen.GenerateWall(testEdge, normal);

                    normal = new(0, 1);
                    testEdge = point + normal * _rectRounding;
                    if (testEdge.y > bounds.Maximum.y && !RectangleTree.TryGetRectangleAtPoint(testEdge, out _, out _))
                        gen.GenerateWall(testEdge, normal);

                    normal = new(0, -1);
                    testEdge = point + normal * _rectRounding;
                    if (testEdge.y < bounds.Minimum.y && !RectangleTree.TryGetRectangleAtPoint(testEdge, out _, out _))
                        gen.GenerateWall(testEdge, normal);
                }
            }
        }


        public interface ICryptGenerator
        {
            void OnNewRoom(RotatedRectangle room);
            void GenerateFloor(Vector2 point);
            void GenerateWall(Vector2 point, Vector2 normal);
        }

        private struct GizmoGenerator : ICryptGenerator
        {
            public float RectRounding;

            public void OnNewRoom(RotatedRectangle room) {}
            public void GenerateFloor(Vector2 point){}
            public void GenerateWall(Vector2 point, Vector2 normal)
            {
                Gizmos.color = Color.orange;
                Gizmos.DrawLine(point.To3D(), (point + normal * RectRounding * .5f).To3D());
            }
        }
    }
}
