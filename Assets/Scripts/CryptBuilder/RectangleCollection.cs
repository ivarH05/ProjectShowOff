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
                Debug.Log("empty tree");
                Add(default);
                Add(BoundingNode.CreateRoot());
            }
            ref var root = ref Nodes[1];
            root.AddRectangle(rect, bounds, this);
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
    }
}