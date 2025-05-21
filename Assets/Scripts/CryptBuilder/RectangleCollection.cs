using System;
using System.Collections.Generic;
using UnityEngine;

namespace CryptBuilder
{
    [Serializable]
    public class RectangleCollection
    {
        public List<BoundingNode> Nodes = new();

        public void AddRectangle(RotatedRectangle rect)
        {
            var bounds = rect.GetBounds();
            if(Nodes.Count < 2)
            {
                Debug.Log("empty tree");
                Nodes.Add(default);
                Nodes.Add(BoundingNode.CreateRoot());
            }
            Nodes[1].AddRectangle(rect, bounds, this);
        }
    }
}