using UnityEngine;
using System.Collections;

public class PursuitBehavior : SteeringBehavior
{
    public float maxSpeed;

    public Rigidbody target;

    public float targetDistance;
    public float estimateArrivalDuration;
    public Vector3 estimatedTargetPosition;

    public Vector3 toTarget;
    public Vector3 toEstimatedTarget;
    public Vector3 desiredVelocity;

    void Update()
    {
        toTarget = target.position - transform.position;
        targetDistance = toTarget.magnitude;
        estimateArrivalDuration = targetDistance / maxSpeed;

        estimatedTargetPosition = target.position + target.velocity * estimateArrivalDuration;
        toEstimatedTarget = estimatedTargetPosition - transform.position;

        desiredVelocity = Vector3.Normalize(toEstimatedTarget) * maxSpeed;
        steeringDirection = desiredVelocity - rb.velocity;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, estimatedTargetPosition);
    }
}
