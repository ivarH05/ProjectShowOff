using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

namespace CryptBuilder
{
    public partial class Builder : MonoBehaviour
    {
        public CryptRoomStyle DefaultStyle => _defaultStyle;
        public float RectRounding => _rectRounding;

        [field:SerializeField] public RectangleCollection RectangleTree {get; private set; }
        [SerializeField] float _rectRounding = .5f;
        [SerializeField] CryptRoomStyle _defaultStyle;
        [SerializeField, Tooltip("Should be a unit quad")] GameObject _floorWallHitbox;

        float _rectRotationRounding = 90;
        bool _showWallGizmos = true;

        private void OnDrawGizmosSelected()
        {
            if (_rectRounding < 0.01f || !_showWallGizmos) 
                return;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
            GizmoGenerator gen = new();
            gen.RectRounding = _rectRounding;
            GenerateCrypt(gen);
            GenerateSurfaces(gen);
        }

        public void GenerateSurfaces<TGenerator>(TGenerator generator) where TGenerator : ICryptSurfaceGenerator
        {
            GenerateSurfacesRecursive(1, ref generator);

            void GenerateSurfacesRecursive(int nodeIndex, ref TGenerator gen)
            {
                var node = RectangleTree.Nodes[nodeIndex];
                var rects = node.Rectangles;
                if (rects != null)
                {
                    for (int i = 0; i < rects.Count; i++)
                    {
                        GenerateSurfaces(nodeIndex, i, ref gen);
                    }
                }
                if (node.ChildAIndex > 0)
                {
                    GenerateSurfacesRecursive(node.ChildAIndex, ref gen);
                    GenerateSurfacesRecursive(node.ChildBIndex, ref gen);
                }
            }
        }

        /// <summary>
        /// Generates the crypt using the given generator.
        /// </summary>
        public void GenerateCrypt<TGenerator>(TGenerator generator) where TGenerator : ICryptTileGenerator
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

        /// <summary>
        /// Generates the tiles for a single room.
        /// </summary>
        /// <param name="nodeIndex">The index of the node.</param>
        /// <param name="rectIndex">The index of the rectangle in the node's rectangle list.</param>
        /// <param name="gen">The generator to use.</param>
        public void GenerateTiles<TGenerator>(int nodeIndex, int rectIndex, ref TGenerator gen) where TGenerator : ICryptTileGenerator
        {
            var higherPriorityRects = GetIntersectingHigherPriorityBoxes(nodeIndex, rectIndex, out var rect, out var bounds);
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

        /// <summary>
        /// Generates the surfaces of a certain room. Floors may overlap - walls don't.
        /// </summary>
        /// <param name="nodeIndex">The index of the node containing the room.</param>
        /// <param name="rectIndex">The index of the rectangle in the node's rectangle list.</param>
        /// <param name="gen">The generator to use.</param>
        public void GenerateSurfaces<TGenerator>(int nodeIndex, int rectIndex, ref TGenerator gen) where TGenerator : ICryptSurfaceGenerator
        {
            var higherPriorityRects = GetIntersectingHigherPriorityBoxes(nodeIndex, rectIndex, out var rect, out var bounds);
            gen.OnNewRoom(rect);

            gen.GenerateFloor(bounds);
            GenerateWall(bounds.Minimum, new(0f, 1f), bounds.Size.y, new(-1,0), ref gen);
            GenerateWall(bounds.Minimum, new(1f, 0f), bounds.Size.x, new(0,-1), ref gen);
            GenerateWall(bounds.Maximum, new(0, -1), bounds.Size.y, new(1,0), ref gen);
            GenerateWall(bounds.Maximum, new(-1, 0), bounds.Size.x, new(0,1), ref gen);

            void GenerateWall(Vector2 start, Vector2 direction, float boundsLength, Vector2 normal, ref TGenerator gen)
            {
                float wallStart = -1;
                bool buildingWallCurrently = false;
                for (float i = .5f * _rectRounding; i < boundsLength; i += _rectRounding)
                {
                    Vector2 point = start + direction * i;
                    bool pointHasWall = true;
                    foreach (var r in higherPriorityRects)
                        if (r.ContainsPoint(point))
                        {
                            pointHasWall = false;
                            break;
                        }

                    var testEdge = point + normal * _rectRounding;
                    pointHasWall = pointHasWall && !RectangleTree.TryGetRectangleAtPoint(testEdge, out _, out _);
                    if (pointHasWall)
                    {
                        if(!buildingWallCurrently)
                        {
                            buildingWallCurrently = true;
                            wallStart = i;
                            continue;
                        }
                    }
                    else
                    {
                        if(buildingWallCurrently)
                        {
                            gen.GenerateWall(start + (wallStart - .5f*_rectRounding)*direction, start + (i - .5f*_rectRounding)*direction, normal);
                            buildingWallCurrently = false;
                        }
                    }
                }
                if(buildingWallCurrently)
                {
                    gen.GenerateWall(start + (wallStart - .5f * _rectRounding) * direction, start + boundsLength * direction, normal);
                }
            }
        }

        List<BoundingBox> GetIntersectingHigherPriorityBoxes(int nodeIndex, int rectIndex, out RotatedRectangle rect, out BoundingBox rectBounds)
        {
            var node = RectangleTree.Nodes[nodeIndex];
            var rects = node.Rectangles;
            rect = rects[rectIndex];
            var bounds = rect.GetBounds();
            rectBounds = bounds;
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
                    (contestCenter.y == thisCenter.y && contestCenter.x > thisCenter.x))
                {
                    higherPriorityRects.Add(contestRect);
                    continue;
                }

                // if there are two literally equal rectangles, idk if i can do anything about it
                // the room will just generate double like whatever
            }
            return higherPriorityRects;
        }

        public interface ICryptSurfaceGenerator
        {
            void OnNewRoom(RotatedRectangle room);
            void GenerateFloor(BoundingBox shape);
            void GenerateWall(Vector2 start, Vector2 end, Vector2 normal);
        }
        public interface ICryptTileGenerator
        {
            void OnNewRoom(RotatedRectangle room);
            void GenerateFloor(Vector2 point);
            void GenerateWall(Vector2 point, Vector2 normal);
        }

        private struct GizmoGenerator : ICryptTileGenerator, ICryptSurfaceGenerator
        {
            public float RectRounding;

            public void OnNewRoom(RotatedRectangle room) 
            {
                Gizmos.color = GetStyleColor(room.Room?.Style);
            }
            public void GenerateFloor(Vector2 point){}
            public void GenerateWall(Vector2 point, Vector2 normal)
            {
                //Gizmos.DrawLine(point.To3D(), (point + normal * RectRounding * .5f).To3D());
            }

            public void GenerateFloor(BoundingBox shape){}

            public void GenerateWall(Vector2 start, Vector2 end, Vector2 normal)
            {
                normal *= .25f*RectRounding;
                Gizmos.DrawLine((start + normal).To3D(), (end + normal).To3D());
            }
        }

        public static Color GetStyleColor(CryptRoomStyle style)
        {
#if UNITY_EDITOR
            if (style == null)
                return Color.gray3;
            else
            {
                var id = GlobalObjectId.GetGlobalObjectIdSlow(style);
                var seed = (uint)id.GetHashCode();
                Unity.Mathematics.Random rand = new(seed);
                float hue = rand.NextFloat();
                return Color.HSVToRGB(hue, 1, .7f);
            }
#else
            return Color.gray3;
#endif
        }
    }
}
