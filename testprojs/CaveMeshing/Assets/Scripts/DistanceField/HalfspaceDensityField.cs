using UnityEngine;

public class HalfspaceDensityField : IDensityField
{
    private readonly Vector3 origin;
    private readonly Vector3 normal;

    public HalfspaceDensityField(Vector3 origin, Vector3 normal)
    {
        this.origin = origin;
        this.normal = normal;
    }

    public float Evaluate(Vector3 position)
    {
        return -Vector3.Dot(position - origin, normal);
    }
}
