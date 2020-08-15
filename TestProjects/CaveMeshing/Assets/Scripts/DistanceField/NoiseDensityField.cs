using UnityEngine;

public class NoiseDensityField : IDensityField
{
    public float Evaluate(Vector3 position)
    {
        var v = (float)SimplexNoise.Noise(position.x, position.y, position.z);
        return (2 * v - 1f);
    }
}
