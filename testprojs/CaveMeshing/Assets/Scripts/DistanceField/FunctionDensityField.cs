using UnityEngine;

public class FunctionDensityField : IDensityField
{
    private readonly System.Func<Vector3, float> func;

    public FunctionDensityField(System.Func<Vector3, float> func)
    {
        this.func = func;
    }

    public float Evaluate(Vector3 position)
    {
        return func(position);
    }
}
