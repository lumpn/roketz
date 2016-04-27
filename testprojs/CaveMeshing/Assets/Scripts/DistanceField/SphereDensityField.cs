using UnityEngine;

public class SphereDensityField : IDensityField
{
    private readonly Vector3 center;
    private readonly float radius;

    public SphereDensityField(Vector3 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public float Evaluate(Vector3 position)
    {
        float density = radius - Vector3.Distance(position, center);
        return density;
    }
}
