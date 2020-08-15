using UnityEngine;

[CreateAssetMenu]
public sealed class RigidbodyObject : ScriptableObject
{
    [SerializeField] private Rigidbody rb;

    public Rigidbody Rigidbody { get { return rb; } set { rb = value; } }
}
