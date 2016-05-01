using UnityEngine;

public class DistortedDensityField : IDensityField
{
    private readonly IDensityField field;
    private readonly Vector3 offset;
    private readonly Vector3 scale;

    public DistortedDensityField(IDensityField field, Vector3 offset, Vector3 scale)
    {
        this.field = field;
        this.offset = offset;
        this.scale = scale;
    }

    public float Evaluate(Vector3 position)
    {
        var pos = Vector3.Scale(position + offset, scale);
        return field.Evaluate(pos);
    }
}
