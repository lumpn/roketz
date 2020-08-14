using UnityEngine;
using System.Collections;

public class Sidewinder : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;

    public float acceleration;
    public float angularVelocity;
    public float gravity;
    public float friction;
    public float maxSlideAcceleration;

    [ReadOnly] public Vector3 deltaTarget;
    [ReadOnly] public Vector3 targetDirection;
    [ReadOnly] public float rightProduct;
    [ReadOnly] public float forwardProduct;
    [ReadOnly] public float steering;

    void FixedUpdate()
    {
        deltaTarget = target.position - rb.position;
        targetDirection = deltaTarget.normalized;

        rightProduct = Vector3.Dot(transform.right, targetDirection);
        forwardProduct = Vector3.Dot(transform.forward, targetDirection);
        if (forwardProduct < 0f)
        {
            // full turn when behind
            rightProduct = (rightProduct < 0) ? -1 : 1;
        }

        // aim to zero
        rb.angularVelocity = transform.up * rightProduct * angularVelocity;

        // full thrust
        rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);

        // anti-slide friction
        {
            var forward = transform.forward;
            var targetVelocity = rb.velocity;
            var flightSpeed = Vector3.Dot(forward, targetVelocity);
            var vParallel = forward * flightSpeed;
            var vOrthogonal = targetVelocity - vParallel;
            var correction = -vOrthogonal / Time.fixedDeltaTime;
            var slideAcceleration = correction * Mathf.Abs(flightSpeed) * friction; // slow speed cause no side friction
            var appliedAcceleration = Vector3.ClampMagnitude(slideAcceleration, maxSlideAcceleration);

            rb.AddForce(appliedAcceleration, ForceMode.Acceleration);
        }

        // gravity
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }
}
