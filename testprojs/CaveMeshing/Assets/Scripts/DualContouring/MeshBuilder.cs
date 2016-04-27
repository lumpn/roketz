using UnityEngine;
using System.Collections.Generic;

namespace DualContouring
{
    public class MeshBuilder
    {
        private static readonly int[,] processEdgeMask = {
            { 3, 2, 1, 0 },
            { 7, 5, 6, 4 },
            { 11, 10, 9, 8 },
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
            { 6, 7, 2 },
        };

        private static readonly int[,] cellProcEdgeMask = {
            { 0, 1, 2, 3, 0 },
            { 4, 5, 6, 7, 0 },
            { 0, 4, 1, 5, 1 },
            { 2, 6, 3, 7, 1 },
            { 0, 2, 4, 6, 2 },
            { 1, 3, 5, 7, 2 },
        };

        private static readonly int[,,] faceProcFaceMask = {
            { { 4, 0, 0 }, { 5, 1, 0 }, { 6, 2, 0 }, { 7, 3, 0 } },
            { { 2, 0, 1 }, { 6, 4, 1 }, { 3, 1, 1 }, { 7, 5, 1 } },
            { { 1, 0, 2 }, { 3, 2, 2 }, { 5, 4, 2 }, { 7, 6, 2 } },
        };

        private static readonly int[,,] edgeProcEdgeMask = {
            { { 3, 2, 1, 0, 0 }, { 7, 6, 5, 4, 0 } },
            { { 5, 1, 4, 0, 1 }, { 7, 3, 6, 2, 1 } },
            { { 6, 4, 2, 0, 2 }, { 7, 5, 3, 1, 2 } },
        };

        private static readonly int[,,] faceProcEdgeMask = {
            { { 1, 4, 0, 5, 1, 1 }, { 1, 6, 2, 7, 3, 1 }, { 0, 4, 6, 0, 2, 2 }, { 0, 5, 7, 1, 3, 2 } },
            { { 0, 2, 3, 0, 1, 0 }, { 0, 6, 7, 4, 5, 0 }, { 1, 2, 0, 6, 4, 2 }, { 1, 3, 1, 7, 5, 2 } },
            { { 1, 1, 0, 3, 2, 0 }, { 1, 5, 4, 7, 6, 0 }, { 0, 1, 5, 0, 4, 1 }, { 0, 3, 7, 2, 6, 1 } },
        };

        private static readonly int[,] faceProcEdgeOrders = {
            { 0, 0, 1, 1 },
            { 0, 1, 0, 1 },
        };

        public readonly List<Vector3> vertices = new List<Vector3>();
        public readonly List<int> triangles = new List<int>();

        public void Build(OctreeNode node)
        {
            vertices.Clear();
            triangles.Clear();

            GenerateVertexIndices(node);
            CellProcContour(node);
        }

        private void GenerateVertexIndices(OctreeNode node)
        {
            if (node == null)
                return;
            
            if (node.IsInternal())
            {
                // recurse for internal nodes
                for (int i = 0; i < node.children.Length; i++)
                {
                    var child = node.children[i];
                    GenerateVertexIndices(child);
                }
            }
            else
            {
                // store vertex index for leaf nodes
                node.vertexIndex = vertices.Count;
                vertices.Add(node.vertexPosition);
            }
        }

        public void CellProcContour(OctreeNode node)
        {
            if (node == null)
                return;

            if (!node.IsInternal())
                return;
            
            // 8 internal cells
            for (int i = 0; i < 8; i++)
            {
                var child = node.children[i];
                CellProcContour(child);
            }

            // 12 internal faces
            for (int i = 0; i < 12; i++)
            {
                // every face is shared by two nodes
                var faceNodes = new OctreeNode[2];

                for (int j = 0; j < 2; j++)
                {
                    int c = cellProcFaceMask[i, j];
                    faceNodes[j] = node.children[c];
                }
                int dir = cellProcFaceMask[i, 2];

                FaceProcContour(faceNodes, dir);
            }

            // 6 internal edges
            for (int i = 0; i < 6; i++)
            {
                // every edge is shared by four nodes
                var edgeNodes = new OctreeNode[4];

                for (int j = 0; j < 4; j++)
                {
                    int c = cellProcEdgeMask[i, j];
                    edgeNodes[j] = node.children[c];
                }

                int dir = cellProcEdgeMask[i, 4];

                EdgeProcContour(edgeNodes, dir);
            }
        }

        private void FaceProcContour(OctreeNode[] nodes, int dir)
        {
            for (int i = 0; i < 2; i++)
            {
                var node = nodes[i];
                if (node == null)
                    return;

                if (!node.IsInternal())
                    return;
            }
        
            // 4 internal faces
            for (int i = 0; i < 4; i++)
            {
                // every face is shared by two nodes
                var subNodes = new OctreeNode[2];

                for (int j = 0; j < 2; j++)
                {
                    int c = faceProcFaceMask[dir, i, j];
                    subNodes[j] = nodes[j].GetChildOrSelf(c);
                }

                int subDir = faceProcFaceMask[dir, i, 2];

                FaceProcContour(subNodes, subDir);
            }

            // 4 internal edges
            var edgeNodes = new OctreeNode[4];
            for (int i = 0; i < 4; i++)
            {
                int k = faceProcEdgeMask[dir, i, 0];
                int edgeDir = faceProcEdgeMask[dir, i, 5];

                for (int j = 0; j < 4; j++)
                {
                    int n = faceProcEdgeOrders[k, j];
                    int c = faceProcEdgeMask[dir, i, j];
                    var node = nodes[n];

                    edgeNodes[j] = node.GetChildOrSelf(c);
                }
                EdgeProcContour(edgeNodes, edgeDir);
            }
        }

        private void EdgeProcContour(OctreeNode[] nodes, int dir)
        {
            bool allInternal = true;
            for (int i = 0; i < 4; i++)
            {
                var node = nodes[i];
                if (node == null)
                    return;
                
                if (!node.IsInternal())
                    allInternal = false;
            }

            if (allInternal)
            {
                // 2 internal edges
                for (int i = 0; i < 2; i++)
                {
                    int edgeDir = edgeProcEdgeMask[dir, i, 4];

                    var edgeNodes = new OctreeNode[4];
                    for (int j = 0; j < 4; j++)
                    {
                        int c = edgeProcEdgeMask[dir, i, j];
                        var node = nodes[j];
                        edgeNodes[j] = node.GetChildOrSelf(c);
                    }
                    EdgeProcContour(edgeNodes, edgeDir);
                }
            }
            else
            {
                ProcessEdge(nodes, dir);
            }
        }

        private void ProcessEdge(OctreeNode[] nodes, int dir)
        {
            int min = -1;
            int minSize = 1000;
            var ind = new []{ -1, -1, -1, -1 };
            var signChange = new[]{ false, false, false, false };
            var flip = new[]{ false, false, false, false };
            int flip2 = -1;

            for (int i = 0; i < 4; i++)
            {
                var node = nodes[i];
                int edge = processEdgeMask[dir, i];

                int c1 = OctreeUtils.edgeCornerIndices[edge, 0];
                int c2 = OctreeUtils.edgeCornerIndices[edge, 1];

                var s1 = (node.cornerFlags >> c1) & 1;
                var s2 = (node.cornerFlags >> c2) & 1;

                if (1 < minSize)
                {
                    minSize = 1;
                    min = i;
                    flip2 = s1;
                }

                ind[i] = node.vertexIndex;
                signChange[i] = (s1 != s2);
            }

            if (signChange[min])
            {
                if (flip2 == 0)
                {
                    if (ind[0] == ind[1])
                    {
                        triangles.Add(ind[0]);
                        triangles.Add(ind[3]);
                        triangles.Add(ind[2]);
                    }
                    else if (ind[1] == ind[3])
                    {
                        triangles.Add(ind[0]);
                        triangles.Add(ind[1]);
                        triangles.Add(ind[2]);
                    }
                    else if (ind[3] == ind[2])
                    {
                        triangles.Add(ind[0]);
                        triangles.Add(ind[1]);
                        triangles.Add(ind[3]);
                    }
                    else if (ind[2] == ind[0])
                    {
                        triangles.Add(ind[1]);
                        triangles.Add(ind[3]);
                        triangles.Add(ind[2]);
                    }
                    else
                    {
                        triangles.Add(ind[0]);
                        triangles.Add(ind[1]);
                        triangles.Add(ind[3]);

                        triangles.Add(ind[0]);
                        triangles.Add(ind[3]);
                        triangles.Add(ind[2]);
                    }
                }
                else
                {
                    if (ind[0] == ind[1])
                    {
                        triangles.Add(ind[0]);
                        triangles.Add(ind[2]);
                        triangles.Add(ind[3]);
                    }
                    else if (ind[1] == ind[3])
                    {
                        triangles.Add(ind[0]);
                        triangles.Add(ind[2]);
                        triangles.Add(ind[1]);
                    }
                    else if (ind[3] == ind[2])
                    {
                        triangles.Add(ind[0]);
                        triangles.Add(ind[3]);
                        triangles.Add(ind[1]);
                    }
                    else if (ind[2] == ind[0])
                    {
                        triangles.Add(ind[1]);
                        triangles.Add(ind[2]);
                        triangles.Add(ind[3]);
                    }
                    else
                    {
                        triangles.Add(ind[0]);
                        triangles.Add(ind[3]);
                        triangles.Add(ind[1]);

                        triangles.Add(ind[0]);
                        triangles.Add(ind[2]);
                        triangles.Add(ind[3]);
                    }
                }
            }
        }
    }
}
