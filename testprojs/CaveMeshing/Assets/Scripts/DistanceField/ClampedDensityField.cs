using UnityEngine;

public class ClampedDensityField : IDensityField
{
    private readonly IDensityField field;
    private readonly float min;
    private readonly float max;

    public ClampedDensityField(IDensityField field, float min, float max)
    {
        this.field = field;
        this.min = min;
        this.max = max;
    }

    public float Evaluate(Vector3 position)
    {
        float v = field.Evaluate(position);
        return Mathf.Clamp(v, min, max);
    }
}
