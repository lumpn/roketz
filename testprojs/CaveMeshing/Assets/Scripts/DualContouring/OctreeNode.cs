using UnityEngine;
using System.Collections.Generic;

namespace DualContouring
{
    public enum OctreeNodeType
    {
        None,
        Internal,
        Leaf,
    }

    public class OctreeNode
    {
        private readonly OctreeNodeType type;

        // internal node
        public readonly OctreeNode[] children;

        // leaf node
        public readonly int cornerFlags;
        public readonly Vector3 vertexPosition;

        // meshing
        public int vertexIndex { get; set; }

        public OctreeNode[] GetChildren()
        {
            return children;
        }

        public Vector3 GetVertexPosition()
        {
            return vertexPosition;
        }

        public OctreeNode(OctreeNode[] children)
        {
            this.type = OctreeNodeType.Internal;
            this.children = children;
        }

        public OctreeNode(Vector3 vertexPosition, int cornerFlags)
        {
            this.type = OctreeNodeType.Leaf;
            this.vertexPosition = vertexPosition;
            this.cornerFlags = cornerFlags;
        }

        public bool IsInternal()
        {
            return (type == OctreeNodeType.Internal);
        }
    }
}
