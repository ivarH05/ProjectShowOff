using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

namespace CryptBuilder
{
    [Serializable]
    public class BoundingNode
    {
        [field: SerializeField] public BoundingNode Parent { get; private set; }
        [field: SerializeField] public BoundingNode ChildA { get; private set; }
        [field: SerializeField] public BoundingNode ChildB { get; private set; }
        public ReadOnlyCollection<RotatedRectangle> Rectangles => _rectangles?.AsReadOnly();
        [field:SerializeField] public BoundingBox Bounds { get; private set; }

        [SerializeField, HideInInspector] List<RotatedRectangle> _rectangles;
        bool _splitVertical;
        float _splitPosition;

        const int SplitThreshold = 4;

        /// <summary>
        /// Recalculates the bounds of the node to get a tighter fit, if possible. This applies to all parents too.
        /// </summary>
        public void RecalculateBoundsUpwardsRecursive()
        {
            RecalculateBoundsSelf();
            Parent?.RecalculateBoundsUpwardsRecursive();
        }

        void RecalculateBoundsSelf()
        {
            if (ChildA == null)
            {
                // no rectangles, no children, just take the bounds of the parent
                if (_rectangles == null || _rectangles.Count == 0)
                {
                    Debug.Log("no rectangles, no bounds");
                    if (Parent == null)
                    {
                        Debug.Log("no parent either?");
                        return;
                    }
                    Bounds = new(Parent.Bounds.Center, Parent.Bounds.Center);
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
        public void AddRectangle(in RotatedRectangle rect, in BoundingBox precomputedBounds)
        {
            var b = Bounds;
            b.GrowToInclude(precomputedBounds);
            Bounds = b;
            if (ChildA == null)
            {
                // this node has not split
                _rectangles ??= new();
                _rectangles.Add(rect);
                if (_rectangles.Count >= SplitThreshold)
                    Split();
            }
            else
            {
                // node has split, send the AddRectangle over to the appropriate child
                bool A = _splitVertical ? precomputedBounds.Center.y < _splitPosition : precomputedBounds.Center.x < _splitPosition;
                if (A)
                    ChildA.AddRectangle(rect, precomputedBounds);
                else ChildB.AddRectangle(rect, precomputedBounds);
            }
        }

        void Split()
        {
            ChildA = new(); ChildA.Parent = this;
            ChildB = new(); ChildB.Parent = this;

            Vector2 thisBoundsSize = Bounds.Size;
            _splitVertical = thisBoundsSize.y > thisBoundsSize.x;
            _splitPosition = _splitVertical ? Bounds.Center.y : Bounds.Center.x;

            var rects = _rectangles;
            _rectangles = null;
            ChildA._rectangles = rects;
            ChildB._rectangles = new();
            float thisBoundsMag = thisBoundsSize.sqrMagnitude;

            Debug.Log("Splitting");
            for(int i = rects.Count-1; i >= 0; i--)
            {
                var rect = rects[i];
                var b = rect.GetBounds();
                if(b.Size.sqrMagnitude*2 > thisBoundsMag)
                {
                    _rectangles ??= new();
                    _rectangles.Add(rect); // in the case that one of the rectangles is MASSIVE, add it to this instead of one of the children.
                    rects.RemoveAt(i);
                    Debug.Log("to SELF");
                    continue;
                }

                float bCenterDist = _splitVertical ? b.Center.y : b.Center.x;
                if (bCenterDist > _splitPosition)
                {
                    rects.RemoveAt(i);
                    ChildB._rectangles.Add(rect);
                    Debug.Log("to B");
                }
                else Debug.Log("to A");
            }
            ChildA.RecalculateBoundsSelf();
            ChildB.RecalculateBoundsSelf();
            RecalculateBoundsUpwardsRecursive();
        }
    }
}
