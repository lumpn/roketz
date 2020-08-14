using UnityEngine;
using System.Collections;

public abstract class SteeringBehavior : MonoBehaviour
{
    public Rigidbody rb;
    public float maxAcceleration;

    [ReadOnly] public Vector3 desiredVelocity;
    [ReadOnly] public Vector3 currentVelocity;
    [ReadOnly] public Vector3 deltaVelocity;

    [ReadOnly] public Vector3 desiredAcceleration;
    [ReadOnly] public Vector3 appliedAcceleration;

    void FixedUpdate()
    {
        currentVelocity = rb.velocity;
        deltaVelocity = desiredVelocity - currentVelocity;

        desiredAcceleration = deltaVelocity / Time.fixedDeltaTime;
        appliedAcceleration = Vector3.ClampMagnitude(desiredAcceleration, maxAcceleration);

        rb.AddForce(appliedAcceleration, ForceMode.Acceleration);
    }
}
