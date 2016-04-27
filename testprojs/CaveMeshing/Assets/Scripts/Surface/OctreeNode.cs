using UnityEngine;
using System.Collections.Generic;

public class OctreeNode
{
    private enum OctreeNodeType
    {
        None,
        Internal,
        Leaf,
    }

    private static readonly Vector3[] childOffset = { 
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 1, 0),
        new Vector3(0, 1, 1),
        new Vector3(1, 0, 0),
        new Vector3(1, 0, 1),
        new Vector3(1, 1, 0),
        new Vector3(1, 1, 1),
    };


    private static readonly int[,] processEdgeMask = {
        { 3, 2, 1, 0 },
        { 7, 5, 6, 4 },
        { 11, 10, 9, 8 }
    };

    private static readonly int[,] cellProcFaceMask = {
        { 0, 4, 0 }, 
        { 1, 5, 0 }, 
        { 2, 6, 0 },
        { 3, 7, 0 },
        { 0, 2, 1 },
        { 4, 6, 1 }, 
        { 1, 3, 1 }, 
        { 5, 7, 1 }, 
        { 0, 1, 2 }, 
        { 2, 3, 2 }, 
        { 4, 5, 2 }, 
        { 6, 7, 2 }
    };

    private static readonly int[,] cellProcEdgeMask = {
        { 0, 1, 2, 3, 0 },
        { 4, 5, 6, 7, 0 },
        { 0, 4, 1, 5, 1 },
        { 2, 6, 3, 7, 1 },
        { 0, 2, 4, 6, 2 },
        { 1, 3, 5, 7, 2 }
    };

    private static readonly int[,,] faceProcFaceMask = {
        { { 4, 0, 0 }, { 5, 1, 0 }, { 6, 2, 0 }, { 7, 3, 0 } },
        { { 2, 0, 1 }, { 6, 4, 1 }, { 3, 1, 1 }, { 7, 5, 1 } },
        { { 1, 0, 2 }, { 3, 2, 2 }, { 5, 4, 2 }, { 7, 6, 2 } }
    };

    private static readonly int[,,] edgeProcEdgeMask = {
        { { 3, 2, 1, 0, 0 }, { 7, 6, 5, 4, 0 } },
        { { 5, 1, 4, 0, 1 }, { 7, 3, 6, 2, 1 } },
        { { 6, 4, 2, 0, 2 }, { 7, 5, 3, 1, 2 } },
    };

    private static readonly int[,,] faceProcEdgeMask = {
        { { 1, 4, 0, 5, 1, 1 }, { 1, 6, 2, 7, 3, 1 }, { 0, 4, 6, 0, 2, 2 }, { 0, 5, 7, 1, 3, 2 } },
        { { 0, 2, 3, 0, 1, 0 }, { 0, 6, 7, 4, 5, 0 }, { 1, 2, 0, 6, 4, 2 }, { 1, 3, 1, 7, 5, 2 } },
        { { 1, 1, 0, 3, 2, 0 }, { 1, 5, 4, 7, 6, 0 }, { 0, 1, 5, 0, 4, 1 }, { 0, 3, 7, 2, 6, 1 } }
    };

    private static readonly int[] edgeVertexIndex0 = {
        0, 1, 2, 3, 0, 1, 4, 6, 0, 2, 4, 6
    };

    private static readonly int[] edgeVertexIndex1 = {
        4, 5, 6, 7, 2, 3, 6, 7, 1, 3, 5, 7
    };

    private OctreeNodeType type;

    // internal node
    private OctreeNode[] children;

    // leaf node
    private int corners;
    private Vector3 vertexPosition;

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


    public static OctreeNode Construct(IDensityFunction densityFunc, Vector3 position, int level)
    {
        if (level == 0)
        {
            return ConstructLeaf(densityFunc, position);
        }

        var childLevel = level - 1;
        var childSize = 1 << childLevel;

        bool hasChildren = false;
        var children = new OctreeNode[8]; // TODO Jonas: cache this
        for (int i = 0; i < 8; i++)
        {
            var childPosition = position + childOffset[i] * childSize;
            var child = Construct(densityFunc, childPosition, childLevel);
            if (child != null)
            {
                children[i] = child;
                hasChildren = true;
            }
        }

        if (!hasChildren)
        {
            return null;
        }

        var node = new OctreeNode();
        node.type = OctreeNodeType.Internal;
        node.children = children; // TODO Jonas: copy from cache

        return node;
    }

    private static readonly float[] densities = new float[8];
    private static readonly Vector3[] cornerPositions = new Vector3[8];

    public static OctreeNode ConstructLeaf(IDensityFunction densityFunc, Vector3 position)
    {
        int leafCorners = 0;
        for (int i = 0; i < 8; i++)
        {
            var cornerPosition = position + childOffset[i];
            cornerPositions[i] = cornerPosition;

            var density = densityFunc.Evaluate(cornerPosition);
            densities[i] = density;

            var isSolid = (density > 0f);
            if (isSolid)
            {
                leafCorners |= (1 << i);
            }
        }

        if (leafCorners == 0 || leafCorners == 255)
        {
            // voxel is fully inside or outside the volume
            return null;
        }

        var averagePosition = Vector3.zero;
        var crossPositions = new Vector3[12];
        int edgecount = 0;
        for (int i = 0; i < 12; i++)
        {
            var v0 = edgeVertexIndex0[i];
            var v1 = edgeVertexIndex1[i];

            var s0 = (leafCorners >> v0) & 1;
            var s1 = (leafCorners >> v1) & 1;

            if (s0 == s1)
            {
                // no zero crossing on this edge
                continue;
            }

            var p0 = cornerPositions[v0];
            var p1 = cornerPositions[v1];

            var d0 = densities[v0];
            var d1 = densities[v1];

            var crossPosition = ((d0 * p1) - (d1 * p0)) / (d0 - d1);
            crossPositions[edgecount] = crossPosition;
            averagePosition += crossPosition;
            edgecount++;
        }

        averagePosition /= edgecount;

        var node = new OctreeNode();
        node.type = OctreeNodeType.Leaf;
        node.children = null;

        node.corners = leafCorners;
        node.vertexPosition = averagePosition;

        return node;
    }

    private static void ContourProcessEdge(OctreeNode[] nodes, int dir, List<int> faces)
    {
        var indizes = new []{ -1, -1, -1, -1 };
        var signChange = new[]{ false, false, false, false };
        bool flip = false;

        for (int i = 0; i < 4; i++)
        {
            var node = nodes[i];
            int edge = processEdgeMask[dir, i];

            int v0 = edgeVertexIndex0[edge];
            int v1 = edgeVertexIndex1[edge];

            var s0 = (node.corners >> v0) & 1;
            var s1 = (node.corners >> v1) & 1;

            if (i == 0)
            {
                flip = (s0 == 0);
            }

            indizes[i] = node.vertexIndex;
            signChange[i] = (s0 != s1);
        }

        if (signChange[0])
        {
            if (flip)
            {
                faces.Add(indizes[0]);
                faces.Add(indizes[1]);
                faces.Add(indizes[3]);

                faces.Add(indizes[0]);
                faces.Add(indizes[3]);
                faces.Add(indizes[2]);
            }
            else
            {
                faces.Add(indizes[0]);
                faces.Add(indizes[3]);
                faces.Add(indizes[1]);

                faces.Add(indizes[0]);
                faces.Add(indizes[2]);
                faces.Add(indizes[3]);
            }
        }
    }

    public void GenerateVertexIndices(List<Vector3> vertices)
    {
        if (type == OctreeNodeType.Leaf)
        {
            vertexIndex = vertices.Count;
            vertices.Add(vertexPosition);
            return;
        }

        for (int i = 0; i < children.Length; i++)
        {
            var child = children[i];
            if (child != null)
            {
                child.GenerateVertexIndices(vertices);
            }
        }
    }

    public void ContourCellProc(List<int> faces)
    {
        if (type == OctreeNodeType.Internal)
        {
            // 8 cells
            for (int i = 0; i < 8; i++)
            {
                var child = children[i];
                if (child != null)
                {
                    child.ContourCellProc(faces);
                }
            }

            // 12 faces
            for (int i = 0; i < 12; i++)
            {
                int c0 = cellProcFaceMask[i, 0];
                int c1 = cellProcFaceMask[i, 1];
                int dir = cellProcFaceMask[i, 2];

                var n0 = children[c0];
                var n1 = children[c1];
                ContourFaceProc(new[]{ n0, n1 }, dir, faces);
            }

            // 6 edges
            for (int i = 0; i < 6; i++)
            {
                var nodes = new OctreeNode[4];

                int c0 = cellProcEdgeMask[i, 0];
                int c1 = cellProcEdgeMask[i, 1];
                int c2 = cellProcEdgeMask[i, 2];
                int c3 = cellProcEdgeMask[i, 3];
                int dir = cellProcEdgeMask[i, 4];

                nodes[0] = children[c0];
                nodes[1] = children[c1];
                nodes[2] = children[c2];
                nodes[3] = children[c3];

                ContourEdgeProc(nodes, dir, faces);
            }
        }
    }

    private static void ContourFaceProc(OctreeNode[] nodes, int dir, List<int>faces)
    {
        var n0 = nodes[0];
        var n1 = nodes[1];

        if (n0 == null || n1 == null)
            return;

        if (n0.type == OctreeNodeType.Internal && n1.type == OctreeNodeType.Internal)
        {
            // 4 faces
            for (int i = 0; i < 4; i++)
            {
                int c0 = faceProcFaceMask[dir, i, 0];
                int c1 = faceProcFaceMask[dir, i, 1];
                int newDir = faceProcFaceMask[dir, i, 2];

                var newN0 = (n0.type == OctreeNodeType.Internal) ? n0.children[c0] : n0;
                var newN1 = (n1.type == OctreeNodeType.Internal) ? n1.children[c1] : n1;

                ContourFaceProc(new[]{ newN0, newN1 }, newDir, faces);
            }

            int[,] orders = {
                { 0, 0, 1, 1 },
                { 0, 1, 0, 1 },
            };

            // 4 edges
            var edgeNodes = new OctreeNode[4];
            for (int i = 0; i < 4; i++)
            {
                int k = faceProcEdgeMask[dir, i, 0];
                int[] cs = {
                    faceProcEdgeMask[dir, i, 1],
                    faceProcEdgeMask[dir, i, 2],
                    faceProcEdgeMask[dir, i, 3],
                    faceProcEdgeMask[dir, i, 4],
                };
                int newDir = faceProcEdgeMask[dir, i, 5];

                for (int j = 0; j < 4; j++)
                {
                    var node = nodes[orders[k, j]];
                    if (node.type != OctreeNodeType.Internal)
                    {
                        edgeNodes[j] = node;
                    }
                    else
                    {
                        edgeNodes[j] = node.children[cs[j]];
                    }
                }
                ContourEdgeProc(edgeNodes, newDir, faces);
            }
        }
    }

    private static void ContourEdgeProc(OctreeNode[] nodes, int dir, List<int> faces)
    {
        bool allInternal = true;
        for (int i = 0; i < 4; i++)
        {
            var node = nodes[i];
            if (node == null)
                return;
            if (node.type != OctreeNodeType.Internal)
                allInternal = false;
        }

        if (allInternal)
        {
            for (int i = 0; i < 2; i++)
            {
                int[] cs = new int[] {
                    edgeProcEdgeMask[dir, i, 0],  
                    edgeProcEdgeMask[dir, i, 1],  
                    edgeProcEdgeMask[dir, i, 2],  
                    edgeProcEdgeMask[dir, i, 3],  
                };
                int edgeDir = edgeProcEdgeMask[dir, i, 4];

                var edgeNodes = new OctreeNode[4];
                for (int j = 0; j < 4; j++)
                {
                    var node = nodes[j];
                    if (node.type == OctreeNodeType.Internal)
                    {
                        edgeNodes[j] = node.children[cs[j]];
                    }
                    else
                    {
                        edgeNodes[j] = node;
                    }
                }
                ContourEdgeProc(edgeNodes, edgeDir, faces);
            }
        }
        else
        {
            ContourProcessEdge(nodes, dir, faces);
        }
    }
}
