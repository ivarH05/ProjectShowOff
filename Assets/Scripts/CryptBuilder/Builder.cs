using System.Collections.Generic;
using UnityEngine;

namespace CryptBuilder
{
    public partial class Builder : MonoBehaviour
    {
        [field:SerializeField] public RectangleCollection RectangleTree {get; private set; }
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
                    Gizmos.DrawLine(new(x,0,y), new(x,.3f,y));
                }
            }

        }
    }
}
