using UnityEngine;

[CreateAssetMenu]
public sealed class FloatObject : ScriptableObject
{
    [SerializeField] private float defaultValue, minValue, maxValue;
    private float currentValue;

    public float Value
    {
        get { return currentValue; }
        set { currentValue = Mathf.Clamp(value, minValue, maxValue); }
    }

    public void Reset()
    {
        Value = defaultValue;
    }
}
