using System;
using System.Collections.Generic;
using UnityEngine;

namespace CryptBuilder
{
    [Serializable]
    public class RectangleCollection
    {
        public BoundingNode[] Nodes = new BoundingNode[2];
        [field:SerializeField, HideInInspector] public int Count { get; private set; }

        public void AddRectangle(RotatedRectangle rect)
        {
            var bounds = rect.GetBounds();
            if(Count < 2)
            {
                Add(default);
                Add(BoundingNode.CreateRoot());
            }
            ref var root = ref Nodes[1];
            root.AddRectangle(rect, bounds, this);
        }

        public void Regenerate(float sizeRounding = 0, float rotationRounding = 0)
        {
            var previousNodes = Nodes;
            Nodes = new BoundingNode[Nodes.Length];
            Count = 0;
            Add(default);
            Add(BoundingNode.CreateRoot(previousNodes[1].Bounds));
            RecursiveReadd(1);

            void RecursiveReadd(int index)
            {
                ref var node = ref previousNodes[index];
                if(node.ChildAIndex > 0)
                {
                    if(UnityEngine.Random.value < .5f)
                    {
                        RecursiveReadd(node.ChildAIndex);
                        RecursiveReadd(node.ChildBIndex);
                    }
                    else
                    {
                        RecursiveReadd(node.ChildBIndex);
                        RecursiveReadd(node.ChildAIndex);
                    }
                }
                if (node.Rectangles != null)
                {
                    foreach (var rect in node.Rectangles)
                    {
                        rect.Round(sizeRounding, rotationRounding);
                        AddRectangle(rect);
                    }
                }
            }
        }

        public void Add(BoundingNode node)
        {
            if(Nodes.Length <= Count)
            {
                var newArray = new BoundingNode[Nodes.Length * 2];
                Nodes.CopyTo(newArray, 0);
                Nodes = newArray;
            }
            Nodes[Count] = node;
            Count++;
        }

        public bool TryGetRectangleAtPoint(Vector2 point, out int nodeIndex, out int rectangleIndex)
        {
            float priority = 0;
            return Nodes[1].TryGetRectangleAtPoint(point, this, ref priority, out rectangleIndex, out nodeIndex);
        }

        public List<(int nodeIndex, int rectIndex)> GetRectanglesIntersectingBox(BoundingBox box)
        {
            List<(int, int)> res = new();
            RecursiveRectBoxSearch(1);

            void RecursiveRectBoxSearch(int nodeIndex)
            {
                var node = Nodes[nodeIndex];

                if (!node.Bounds.Intersects(box)) return;

                var rects = node.Rectangles;
                if(rects != null)
                    for (int i = 0; i < rects.Count; i++)
                        if (rects[i].GetBounds().Intersects(box))
                            res.Add((nodeIndex, i));

                if(node.ChildAIndex > 0)
                {
                    RecursiveRectBoxSearch(node.ChildAIndex);
                    RecursiveRectBoxSearch(node.ChildBIndex);
                }
            }

            return res;
        }
    }
}