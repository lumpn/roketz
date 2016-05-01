using UnityEngine;
using System.Collections;

public class CombinedDensityField : IDensityField
{
    public enum Operation
    {
        Minimum,
        Maximum,
        Mix,
        Add,
        Subtract,
        Multiply,
    }

    private readonly IDensityField fieldA;
    private readonly IDensityField fieldB;
    private readonly Operation operation;

    public CombinedDensityField(IDensityField fieldA, IDensityField fieldB, Operation operation)
    {
        this.fieldA = fieldA;
        this.fieldB = fieldB;
        this.operation = operation;
    }

    public float Evaluate(Vector3 position)
    {
        float a = fieldA.Evaluate(position);
        float b = fieldB.Evaluate(position);

        switch (operation)
        {
            case Operation.Minimum:
                return Mathf.Min(a, b);
            case Operation.Maximum:
                return Mathf.Max(a, b);
            case Operation.Mix:
                return Mathf.Min(a, b) + Mathf.Max(a, b);
            case Operation.Add:
                return (a + b);
            case Operation.Subtract:
                return (a - b);
            case Operation.Multiply:
                return (a * b);
        }

        return 0f;
    }
}
