using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

namespace CryptBuilder
{
    [Serializable]
    public struct BoundingNode
    {
        public int ParentIndex => _parentIndex;
        public int ThisIndex => _thisIndex;
        public int ChildAIndex => _childAIndex;
        public int ChildBIndex => _childBIndex;
        public BoundingBox Bounds => _bounds;

        [SerializeField] int _parentIndex;
        [SerializeField] int _thisIndex;
        [SerializeField] int _childAIndex;
        [SerializeField] int _childBIndex;
        public ReadOnlyCollection<RotatedRectangle> Rectangles => _rectangles?.AsReadOnly();
        [SerializeField] BoundingBox _bounds;

        [SerializeField] List<RotatedRectangle> _rectangles;
        bool _splitVertical;
        float _splitPosition;

        const int SplitThreshold = 4;

        public static BoundingNode CreateRoot(BoundingBox initialBounds = default)
        {
            var res = new BoundingNode();
            res._thisIndex = 1;
            res._bounds = initialBounds;
            return res;
        }

        /// <summary>
        /// Recalculates the bounds of the node to get a tighter fit, if possible. This applies to all parents too.
        /// </summary>
        public void RecalculateBoundsUpwardsRecursive(RectangleCollection owner)
        {
            RecalculateBoundsSelf(owner);
            if(_parentIndex > 0)
                owner.Nodes[_parentIndex].RecalculateBoundsUpwardsRecursive(owner);
        }

        void RecalculateBoundsSelf(RectangleCollection owner)
        {
            if (_childAIndex < 1)
            {
                // no rectangles, no children, just take the bounds of the parent
                if (_rectangles == null || _rectangles.Count == 0)
                {
                    if (_parentIndex < 1)
                    {
                        Debug.Log("bounding node missing rectangles and parent while resizing");
                        return;
                    }
                    var parent = owner.Nodes[_parentIndex];
                    _bounds = new(parent._bounds.Center, parent._bounds.Center);
                    return;
                }
                Vector2 min = QuickVec.Max;
                Vector2 max = QuickVec.Min;
                foreach (RotatedRectangle rect in _rectangles)
                {
                    var b = rect.GetBounds();
                    min = Vector2.Min(min, b.Minimum);
                    max = Vector2.Max(max, b.Maximum);
                }
                _bounds = new(min, max);
            }
            else
            {
                ref var ChildA = ref owner.Nodes[_childAIndex];
                ref var ChildB = ref owner.Nodes[_childBIndex];
                var b = ChildA._bounds;
                b.GrowToInclude(ChildB._bounds);
                if (_rectangles != null)
                {
                    foreach (var rect in _rectangles)
                        b.GrowToInclude(rect.GetBounds());
                }
                _bounds = b;
            }
        }

        /// <summary>
        /// Adds a rectangle to the BoundingNode.
        /// </summary>
        /// <param name="rect">The rectangle to add.</param>
        /// <param name="precomputedBounds">The bounds of the rectangle to add.</param>
        public void AddRectangle(in RotatedRectangle rect, in BoundingBox precomputedBounds, RectangleCollection owner)
        {
            var b = _bounds;
            b.GrowToInclude(precomputedBounds);
            _bounds = b;
            if (_childAIndex < 1)
            {
                // this node has not split
                _rectangles ??= new();
                _rectangles.Add(rect);
                if (_rectangles.Count >= SplitThreshold)
                    Split(owner);
            }
            else
            {
                // node has split, send the AddRectangle over to the appropriate child
                bool A = _splitVertical ? precomputedBounds.Center.y < _splitPosition : precomputedBounds.Center.x < _splitPosition;
                ref var child = ref owner.Nodes[A ? _childAIndex : _childBIndex];
                child.AddRectangle(rect, precomputedBounds, owner);
            }
        }

        /// <summary>
        /// Removes a rectangle at a certain index.
        /// </summary>
        /// <param name="rectangleIndex">The index of the rectangle to remove.</param>
        public void RemoveRectangle(int rectangleIndex, RectangleCollection owner)
        {
            _rectangles.RemoveAt(rectangleIndex);
            RecalculateBoundsUpwardsRecursive(owner);
        }

        /// <summary>
        /// Tries to hit test the bounding node and its children.
        /// </summary>
        /// <param name="point">The point to test for.</param>
        /// <param name="rectIndex">The index of the rectangle, if found.</param>
        /// <param name="nodeIndex">The index of the node that contains the found rectangle.</param>
        /// <returns>Whether or not the point hit a rectangle.</returns>
        public bool TryGetRectangleAtPoint(Vector2 point, RectangleCollection owner, ref float currentPriority, out int rectIndex, out int nodeIndex)
        {
            rectIndex = default;
            nodeIndex = default;
            bool result = false;

            if (_rectangles != null)
            {
                for (int i = 0; i < _rectangles.Count; i++)
                {
                    var rect = _rectangles[i];
                    var bounds = rect.GetBounds();
                    float priority = bounds.Size.x * bounds.Size.y;
                    if (priority > currentPriority && bounds.ContainsPoint(point) && rect.ContainsPoint(point))
                    {
                        rectIndex = i;
                        nodeIndex = ThisIndex;
                        currentPriority = priority;
                        result = true;
                    }
                }
            }

            if(_childAIndex > 0)
            {
                ref var ChildA = ref owner.Nodes[ChildAIndex];
                var childABounds = ChildA.Bounds;
                if (childABounds.Size.x * childABounds.Size.y > currentPriority &&
                    childABounds.ContainsPoint(point) && 
                    ChildA.TryGetRectangleAtPoint(point, owner, ref currentPriority, out int rectA, out int nodeA))
                {
                    result = true;
                    rectIndex = rectA;
                    nodeIndex = nodeA;
                }

                ref var ChildB = ref owner.Nodes[ChildBIndex];
                var childBBounds = ChildB.Bounds;
                if (childBBounds.Size.x * childBBounds.Size.y > currentPriority &&
                    childBBounds.ContainsPoint(point) &&
                    ChildB.TryGetRectangleAtPoint(point, owner, ref currentPriority, out int rectB, out int nodeB))
                {
                    result = true;
                    rectIndex = rectB;
                    nodeIndex = nodeB;
                }
            }
            return result;
        }

        void Split(RectangleCollection owner)
        {
            _childAIndex = owner.Count;
            owner.Add(default);
            _childBIndex = owner.Count;
            owner.Add(default);
            owner.Nodes[ThisIndex] = this; // fix for list rescale
            ref var ChildA = ref owner.Nodes[_childAIndex];             
            ref var ChildB = ref owner.Nodes[_childBIndex];

            ChildA._parentIndex = _thisIndex;
            ChildB._parentIndex = _thisIndex;
            ChildA._thisIndex = ChildAIndex;
            ChildB._thisIndex = ChildBIndex;

            Vector2 thisBoundsSize = _bounds.Size;
            _splitVertical = thisBoundsSize.y > thisBoundsSize.x;
            _splitPosition = _splitVertical ? _bounds.Center.y : _bounds.Center.x;

            var rects = _rectangles;
            _rectangles = null;
            ChildA._rectangles = rects;
            ChildB._rectangles = new();
            float thisBoundsMag = thisBoundsSize.sqrMagnitude;

            for(int i = rects.Count-1; i >= 0; i--)
            {
                var rect = rects[i];
                var b = rect.GetBounds();
                if(b.Size.sqrMagnitude*2 > thisBoundsMag)
                {
                    _rectangles ??= new();
                    _rectangles.Add(rect); // in the case that one of the rectangles is MASSIVE, add it to this instead of one of the children.
                    rects.RemoveAt(i);
                    continue;
                }

                float bCenterDist = _splitVertical ? b.Center.y : b.Center.x;
                if (bCenterDist > _splitPosition)
                {
                    rects.RemoveAt(i);
                    ChildB._rectangles.Add(rect);
                }
            }

            ChildA.RecalculateBoundsSelf(owner);
            ChildB.RecalculateBoundsSelf(owner);
            RecalculateBoundsUpwardsRecursive(owner);
            owner.Nodes[ThisIndex] = this; // fix for list rescale
        }
    }
}
