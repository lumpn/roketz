using UnityEngine;
using System.Collections;

public abstract class SteeringBehavior : MonoBehaviour
{
    public Rigidbody rb;
    public float maxForce;

    public Vector3 steeringDirection;

    public Vector3 steeringForce;

    void FixedUpdate()
    {
        steeringForce = Vector3.ClampMagnitude(steeringDirection, maxForce);

        rb.AddForce(steeringForce);
    }
}
