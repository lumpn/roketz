using UnityEngine;

public sealed class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform targetPosition;

    void OnTriggerEnter(Collider coll)
    {
        var rb = coll.attachedRigidbody;
        rb.position = targetPosition.position;
    }
}
