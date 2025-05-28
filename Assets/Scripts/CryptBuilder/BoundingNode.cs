using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

namespace CryptBuilder
{
    [Serializable]
    public struct BoundingNode
    {
        [field: SerializeField] public int ParentIndex { get; private set; }
        [field: SerializeField] public int ThisIndex { get; private set; }
        [field: SerializeField] public int ChildAIndex { get; private set; }
        [field: SerializeField] public int ChildBIndex { get; private set; }
        public ReadOnlyCollection<RotatedRectangle> Rectangles => _rectangles?.AsReadOnly();
        [field:SerializeField] public BoundingBox Bounds { get; private set; }

        [SerializeField] List<RotatedRectangle> _rectangles;
        bool _splitVertical;
        float _splitPosition;

        const int SplitThreshold = 4;

        public static BoundingNode CreateRoot()
        {
            var res = new BoundingNode();
            res.ThisIndex = 1;
            return res;
        }

        /// <summary>
        /// Recalculates the bounds of the node to get a tighter fit, if possible. This applies to all parents too.
        /// </summary>
        public void RecalculateBoundsUpwardsRecursive(RectangleCollection owner)
        {
            RecalculateBoundsSelf(owner);
            if(ParentIndex > 0)
                owner.Nodes[ParentIndex].RecalculateBoundsUpwardsRecursive(owner);
        }

        void RecalculateBoundsSelf(RectangleCollection owner)
        {
            if (ChildAIndex < 1)
            {
                // no rectangles, no children, just take the bounds of the parent
                if (_rectangles == null || _rectangles.Count == 0)
                {
                    Debug.Log("no rectangles, no bounds");
                    if (ParentIndex < 1)
                    {
                        Debug.Log("no parent either?");
                        return;
                    }
                    var parent = owner.Nodes[ParentIndex];
                    Bounds = new(parent.Bounds.Center, parent.Bounds.Center);
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
                Bounds = new(min, max);
            }
            else
            {
                var ChildA = owner.Nodes[ChildAIndex];
                var ChildB = owner.Nodes[ChildBIndex];
                var b = ChildA.Bounds;
                b.GrowToInclude(ChildB.Bounds);
                if (_rectangles != null)
                {
                    foreach (var rect in _rectangles)
                        b.GrowToInclude(rect.GetBounds());
                }
                Bounds = b;
            }
        }

        /// <summary>
        /// Adds a rectangle to the BoundingNode.
        /// </summary>
        /// <param name="rect">The rectangle to add.</param>
        /// <param name="precomputedBounds">The bounds of the rectangle to add.</param>
        public void AddRectangle(in RotatedRectangle rect, in BoundingBox precomputedBounds, RectangleCollection owner)
        {
            var b = Bounds;
            b.GrowToInclude(precomputedBounds);
            Bounds = b;
            if (ChildAIndex < 1)
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
                if (A)
                    owner.Nodes[ChildAIndex].AddRectangle(rect, precomputedBounds, owner);
                else owner.Nodes[ChildBIndex].AddRectangle(rect, precomputedBounds, owner);
            }
        }

        void Split(RectangleCollection owner)
        {
            var ChildA = new BoundingNode(); ChildA.ParentIndex = ThisIndex;
            var ChildB = new BoundingNode(); ChildB.ParentIndex = ThisIndex;

            Vector2 thisBoundsSize = Bounds.Size;
            _splitVertical = thisBoundsSize.y > thisBoundsSize.x;
            _splitPosition = _splitVertical ? Bounds.Center.y : Bounds.Center.x;

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

            ChildAIndex = owner.Nodes.Count;
            ChildA.ThisIndex = owner.Nodes.Count;
            owner.Nodes.Add(ChildA);
            ChildBIndex = owner.Nodes.Count;
            ChildB.ThisIndex = owner.Nodes.Count;
            owner.Nodes.Add(ChildB);

            owner.Nodes[ChildAIndex].RecalculateBoundsSelf(owner);
            owner.Nodes[ChildBIndex].RecalculateBoundsSelf(owner);
            RecalculateBoundsUpwardsRecursive(owner);
        }
    }
}
