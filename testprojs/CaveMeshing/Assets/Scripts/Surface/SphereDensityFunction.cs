using UnityEngine;

public class SphereDensityFunction : IDensityFunction
{
    private readonly Vector3 center;
    private readonly float radius;

    public SphereDensityFunction(Vector3 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public float Evaluate(Vector3 position)
    {
        return radius - Vector3.Distance(position, center);
    }
}
