using UnityEngine;

public class CaveMesher : MonoBehaviour
{
    public Texture2D densityMap;
    public int xSamples = 100;
    public int ySamples = 100;
    public int zSamples = 10;
    public int zMid = 2;
    public bool invert = false;

    public AnimationCurve minThreshold;
    public AnimationCurve maxThreshold;

    public GameObject cubePrefab;

    public Material playgroundMaterial;
    public Material backgroundMaterial;
    public Material foregroundMaterial;

    private GameObject root;

    void Start ()
    {
        Generate ();
    }

    public void Generate ()
    {
        GameObjectUtils.Destroy (root);

        root = new GameObject ("root");
        root.transform.parent = transform;

        int xMid = xSamples / 2;
        int yMid = ySamples / 2;

        for (int x = 0; x < xSamples; x++) {
            for (int y = 0; y < ySamples; y++) {

                var px = x * densityMap.width / xSamples;
                var py = y * densityMap.height / ySamples;

                var color = densityMap.GetPixel (px, py);

                for (int z = 0; z < zSamples; z++) {

                    float f = (float)z / zSamples;
                    float aMin = minThreshold.Evaluate (f);
                    float aMax = maxThreshold.Evaluate (f);

                    bool inside = (color.a >= aMin && color.a <= aMax);
                    if (invert) {
                        inside = (color.a > aMax);
                    }

                    if (inside) {

                        var material = playgroundMaterial;
                        if (z < zMid)
                            material = foregroundMaterial;
                        else if (z > zMid)
                            material = backgroundMaterial;

                        var cube = Object.Instantiate<GameObject> (cubePrefab);
                        cube.GetComponent<MeshRenderer> ().sharedMaterial = material;

                        cube.transform.parent = root.transform;
                        cube.transform.position = new Vector3 (x - xMid, y - yMid, z - zMid);
                    }
                }
            }
        }
    }
}
