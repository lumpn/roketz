using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public sealed class FloatObject : ScriptableObject
{
    [SerializeField] private float defaultValue, minValue, maxValue;

    private float currentValue;

    private readonly List<IFloatListener> listeners = new List<IFloatListener>();

    public float Value
    {
        get { return currentValue; }
        set
        {
            var oldValue = currentValue;
            currentValue = Mathf.Clamp(value, minValue, maxValue);

            foreach (var listener in listeners)
            {
                try
                {
                    listener.OnValueChanged(this, oldValue, currentValue);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex, this);
                }
            }
        }
    }

    public void Reset()
    {
        Value = defaultValue;
    }

    public void AddListener(IFloatListener listener)
    {
        if (listeners.Contains(listener)) return;
        listeners.Add(listener);
    }

    public void RemoveListener(IFloatListener listener)
    {
        var idx = listeners.IndexOf(listener);
        if (idx < 0) return;

        var last = listeners.Count - 1;
        listeners[idx] = listeners[last];
        listeners.RemoveAt(last);
    }
}
