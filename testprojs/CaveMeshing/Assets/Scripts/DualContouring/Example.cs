using UnityEngine;
using System.Collections.Generic;

namespace DualContouring
{
    public class Example : MonoBehaviour
    {
        public GameObject vertexPrefab;

        public MeshFilter meshFilter;

        public Vector3 origin;
        public int level;

        public Color[] gizmoColors;

        private OctreeNode octree;

        void Start()
        {
            var sphere = new ScaledDensityField(new SphereDensityField(new Vector3(12, 12, 12), 10f), 1f);
            var halfspace = new ScaledDensityField(new HalfspaceDensityField(new Vector3(12, 12, 12), new Vector3(0, 1, 0)), 1f);
            var densityField = new CombinedDensityField(sphere, halfspace, CombinedDensityField.Operation.Maximum);

//            System.Func<float, float, float> func = (x, z) => ((x*x + z*z) + 1- ((1-x)*(1-x) + (1-z)*(1-z)));
//            System.Func<float, float, float> func = (x, z) => Mathf.Lerp(Mathf.Min(x, z), Mathf.Max(x, z), ((x+1)/2 + (z+1)/2) / 2);
//            System.Func<float, float, float> func = (x, z) => (Mathf.Lerp(x * z, 1 - ((1 - x) * (1 - z)), (x + z) / 2));
//            System.Func<Vector3,float> adapter = (v) => (8 + 8 * func(v.x / 8 - 1, v.z / 8 - 1) - v.y);
//            System.Func<Vector3, float> adapter = (v) => (10f - v.y);
//            var densityField = new FunctionDensityField(adapter);

            var octreeBuilder = new OctreeBuilder();
            octree = octreeBuilder.Construct(densityField, origin, level);

            //DrawOctree(octree);

            var meshBuilder = new MeshBuilder();
            meshBuilder.Build(octree);

            Debug.LogFormat("Dual contouring generated {0} vertices", meshBuilder.vertices.Count);
            Debug.LogFormat("Dual contouring generated {0} vertex references", meshBuilder.triangles.Count);

            var mesh = new Mesh();
            mesh.vertices = meshBuilder.vertices.ToArray();
            mesh.triangles = meshBuilder.triangles.ToArray();
            meshFilter.mesh = mesh;
        }

        private void DrawOctree(OctreeNode node)
        {
            if (node == null)
                return;

            var children = node.GetChildren();
            if (children == null)
            {
                var vertex = Object.Instantiate<GameObject>(vertexPrefab);
                vertex.transform.position = node.GetVertexPosition();
            }
            else
            {
                for (int i = 0; i < children.Length; i++)
                {
                    DrawOctree(children[i]);
                }
            }
        }

        void OnDrawGizmos()
        {
            if (octree == null)
                return;

            DrawOctreeGizmos(octree, origin, level);
        }

        private void DrawOctreeGizmos(OctreeNode node, Vector3 position, int nodeLevel)
        {
            if (node == null)
                return;
            
            var nodeSize = 1 << nodeLevel;

            var scale = Vector3.one * nodeSize;
            Gizmos.color = gizmoColors[nodeLevel];
            Gizmos.DrawWireCube(position + (scale / 2), scale * 0.99f);

            if (nodeLevel > 0)
            {
                // subdivide
                var childLevel = nodeLevel - 1;
                var childSize = 1 << childLevel;

                for (int i = 0; i < 8; i++)
                {
                    var childPosition = position + OctreeUtils.childOffsets[i] * childSize;
                    DrawOctreeGizmos(node.children[i], childPosition, childLevel);
                }
            }
        }
    }
}