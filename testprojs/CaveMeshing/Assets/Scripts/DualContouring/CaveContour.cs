using UnityEngine;

namespace DualContouring
{
    public class CaveContour : MonoBehaviour
    {
        public Texture2D densityMap;
        public MeshFilter meshFilter;


        public AnimationCurve threshold;
        public AnimationCurve noiseCurve;
        public float zScale = 1f;
        public float noiseScale = 0.1f;

        public Vector3 octreeOrigin;
        public int level;

        public Color[] gizmoColors;

        private OctreeNode octree;

        private float textureScale;
        private float thresholdScale;

        private IDensityField noiseField;

        void Start()
        {
            var size = 1 << level;
            textureScale = (float)Mathf.Max(densityMap.width, densityMap.height) / size;
            thresholdScale = 1f / size;

            noiseField = new DistortedDensityField(new NoiseDensityField(), Vector3.zero, Vector3.one * noiseScale);


            var densityField = new FunctionDensityField(EvaluateTexture);

            var octreeBuilder = new OctreeBuilder();
            octree = octreeBuilder.Construct(densityField, octreeOrigin, level);

            var meshBuilder = new MeshBuilder();
            meshBuilder.Build(octree);

            Debug.LogFormat("Dual contouring generated {0} vertices", meshBuilder.vertices.Count);
            Debug.LogFormat("Dual contouring generated {0} vertex references", meshBuilder.triangles.Count);

            var mesh = new Mesh();
            mesh.vertices = meshBuilder.vertices.ToArray();
            mesh.triangles = meshBuilder.triangles.ToArray();
            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;
        }

        private float EvaluateTexture(Vector3 position)
        {
            var px = position * textureScale;
            var color = densityMap.GetPixel(Mathf.FloorToInt(px.x), Mathf.FloorToInt(px.y));
            var noise = noiseField.Evaluate(position);

            var q = noiseCurve.Evaluate(position.z * zScale * thresholdScale);
            var w = Mathf.Lerp(color.a, noise, q);

            var v = threshold.Evaluate(position.z * zScale * thresholdScale);

            return (v - w);
        }

        void OnDrawGizmos()
        {
            DrawOctreeGizmos(octree, octreeOrigin, level);
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