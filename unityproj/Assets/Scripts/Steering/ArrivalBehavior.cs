using UnityEngine;
using System.Collections;

public class ArrivalBehavior : SteeringBehavior
{
    public float maxSpeed;
    public float slowingDistance;

    public Transform target;

    public float targetDistance;
    public float rampedSpeed;
    public float clippedSpeed;
    public Vector3 toTarget;
    public Vector3 desiredVelocity;

    void Update()
    {
        toTarget = target.position - transform.position;
        targetDistance = toTarget.magnitude;
        rampedSpeed = maxSpeed * (targetDistance / slowingDistance);
        clippedSpeed = Mathf.Min(rampedSpeed, maxSpeed);

        desiredVelocity = Vector3.Normalize(toTarget) * clippedSpeed;
        steeringDirection = desiredVelocity - rb.velocity;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, target.position);
    }
}
