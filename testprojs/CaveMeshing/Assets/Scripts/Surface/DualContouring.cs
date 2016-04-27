using UnityEngine;
using System.Collections.Generic;

public class DualContouring : MonoBehaviour
{
    public GameObject vertexPrefab;

    public MeshFilter meshFilter;

    void Start()
    {
        var densityFunc = new SphereDensityFunction(new Vector3(6, 6, 6), 5f);

        var node = OctreeNode.Construct(densityFunc, Vector3.zero, 4);

        DrawOctree(node);

        var mesh = GenerateMesh(node);
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

    private Mesh GenerateMesh(OctreeNode node)
    {
        var vertices = new List<Vector3>();
        var faces = new List<int>();

        node.GenerateVertexIndices(vertices);
        Debug.LogFormat("Dual contouring generated {0} vertices", vertices.Count);

        node.ContourCellProc(faces);
        Debug.LogFormat("Dual contouring generated {0} vertex references", faces.Count);

        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = faces.ToArray();
        return mesh;
    }
}
