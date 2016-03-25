using UnityEngine;
using System.Collections;

public class Sidewinder : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;

    public float maxSpeed;
    public float angVel;
    public float gravity;
    public float friction;

    public Vector3 toTarget;
    public Vector3 desiredDirection;
    public float dotProduct;
    public float steering;

    void FixedUpdate()
    {
        toTarget = target.position - rb.position;
        desiredDirection = Vector3.Normalize(toTarget);
        dotProduct = Vector3.Dot(transform.right, desiredDirection);
        var behind = Vector3.Dot(transform.forward, desiredDirection);
        if (behind < 0f) {
            // full turn when behind
            if (dotProduct < 0f) {
                dotProduct = -1f;
            } else {
                dotProduct = 1f;
            }
        }

        rb.AddForce(transform.forward * maxSpeed); // full thrust

        // anti-slide friction
        var v1 = transform.forward;
        var v0 = rb.velocity;
        var flightSpeed = Vector3.Dot(v0, v1);
        var vParallel = v1 * flightSpeed;
        var vOrthogonal = v0 - vParallel;
        var correction = -vOrthogonal / Time.fixedDeltaTime;
        var acceleration = correction * Mathf.Abs(flightSpeed) * friction; // slow speed cause no side friction

        rb.AddForce(acceleration, ForceMode.Acceleration);

        rb.angularVelocity = transform.up * dotProduct * angVel;

        // gravity
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }
}
