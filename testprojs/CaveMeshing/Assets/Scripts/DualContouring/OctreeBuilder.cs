using UnityEngine;

namespace DualContouring
{
    /// Octree builder
    /// 
    /// y
    /// | z
    /// |/
    /// +-----x
    public class OctreeBuilder
    {
        private readonly Vector3[] cachePositions = new Vector3[8];
        private readonly float[] cacheDensities = new float[8];

        public OctreeNode Construct(IDensityField densityField, Vector3 position, int level)
        {
            // leaf?
            if (level == 0)
            {
                return ConstructLeaf(densityField, position);
            }

            // subdivide
            var childLevel = level - 1;
            var childSize = 1 << childLevel;

            var children = new OctreeNode[8];
            bool hasChildren = false;
            for (int i = 0; i < 8; i++)
            {
                var childPosition = position + OctreeUtils.childOffsets[i] * childSize;
                var child = Construct(densityField, childPosition, childLevel);
                if (child != null)
                {
                    children[i] = child;
                    hasChildren = true;
                }
            }

            // empty?
            if (!hasChildren)
            {
                return null;
            }

            // construct internal node
            var node = new OctreeNode(children);
            return node;
        }

        public OctreeNode ConstructLeaf(IDensityField densityField, Vector3 position)
        {
            // evaluate density at corners
            int cornerFlags = 0;
            for (int i = 0; i < 8; i++)
            {
                var cornerPosition = position + OctreeUtils.childOffsets[i];
                cachePositions[i] = cornerPosition;

                var density = densityField.Evaluate(cornerPosition);
                cacheDensities[i] = density;

                var isSolid = (density > 0f);
                if (isSolid)
                {
                    cornerFlags |= (1 << i);
                }
            }

            if (cornerFlags == 0 || cornerFlags == 255)
            {
                // voxel is fully inside or outside the volume
                return null;
            }

            // compute zero density crossings
            var averagePosition = Vector3.zero;
            int crossings = 0;
            for (int i = 0; i < 12; i++)
            {
                var c0 = OctreeUtils.edgeCornerIndices[i, 0];
                var c1 = OctreeUtils.edgeCornerIndices[i, 1];

                var f0 = (cornerFlags >> c0) & 1;
                var f1 = (cornerFlags >> c1) & 1;

                if (f0 == f1)
                {
                    // no zero crossing on this edge
                    continue;
                }

                var p0 = cachePositions[c0];
                var p1 = cachePositions[c1];

                var d0 = cacheDensities[c0];
                var d1 = cacheDensities[c1];

                // approximate zero density crossing by linear interpolation
                var crossPosition = ((d0 * p1) - (d1 * p0)) / (d0 - d1);
                averagePosition += crossPosition;
                crossings++;
            }

            averagePosition /= crossings;

            var node = new OctreeNode(averagePosition, cornerFlags);
            return node;
        }
    }
}
