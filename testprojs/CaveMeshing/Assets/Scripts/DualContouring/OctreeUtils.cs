using UnityEngine;

namespace DualContouring
{
    public static class OctreeUtils
    {
        /// corner layout
        ///    3 ---- 7
        ///   /      /|
        ///  2 ---- 6 |
        ///  |      | 5
        ///  |      |/
        ///  0 ---- 4
        public static readonly int[,] edgeCornerIndices = {
            { 0, 4 }, { 1, 5 }, { 2, 6 }, { 3, 7 },    // x-axis 
            { 0, 2 }, { 1, 3 }, { 4, 6 }, { 5, 7 },    // y-axis
            { 0, 1 }, { 2, 3 }, { 4, 5 }, { 6, 7 },    // z-axis
        };


        public static OctreeNode GetChildOrSelf(this OctreeNode node, int childIndex)
        {
            if (node.IsInternal())
            {
                return node.children[childIndex];
            }

            return node;
        }
    }
}
