using UnityEngine;
using System.Collections.Generic;

namespace DualContouring
{
    public class Example : MonoBehaviour
    {
        public GameObject vertexPrefab;

        public MeshFilter meshFilter;

        void Start()
        {
            var densityField = new SphereDensityField(new Vector3(6, 6, 6), 5f);

            var octreeBuilder = new OctreeBuilder();
            var node = octreeBuilder.Construct(densityField, Vector3.zero, 4);

            DrawOctree(node);

            var meshBuilder = new MeshBuilder();
            meshBuilder.Build(node);

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
    }
}