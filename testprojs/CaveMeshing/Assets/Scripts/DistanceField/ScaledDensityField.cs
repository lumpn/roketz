using UnityEngine;

public class ScaledDensityField : IDensityField
{
    private readonly IDensityField field;
    private readonly float scale;

    public ScaledDensityField(IDensityField field, float scale)
    {
        this.field = field;
        this.scale = scale;
    }

    public float Evaluate(Vector3 position)
    {
        float v = field.Evaluate(position);
        return (v * scale);
    }
}
