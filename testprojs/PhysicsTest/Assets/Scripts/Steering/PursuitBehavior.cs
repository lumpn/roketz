using UnityEngine;
using System.Collections;

public class PursuitBehavior : SteeringBehavior
{
    public Rigidbody target;

    public float maxSpeed;

    [ReadOnly] public Vector3 deltaTarget;
    [ReadOnly] public float targetDistance;
    [ReadOnly] public float estimateArrivalDuration;
    [ReadOnly] public Vector3 estimatedTargetPosition;

    [ReadOnly] public Vector3 deltaEstimatedTarget;
    [ReadOnly] public Vector3 estimatedTargetDirection;

    void Update()
    {
        deltaTarget = target.position - transform.position;
        targetDistance = deltaTarget.magnitude;
        estimateArrivalDuration = targetDistance / maxSpeed;

        estimatedTargetPosition = target.position + target.velocity * estimateArrivalDuration;
        deltaEstimatedTarget = estimatedTargetPosition - transform.position;
        estimatedTargetDirection = deltaEstimatedTarget.normalized;

        desiredVelocity = estimatedTargetDirection * maxSpeed;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, estimatedTargetPosition);
    }
}
