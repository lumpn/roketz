using UnityEngine;
using System.Collections;

public class ArrivalBehavior : SteeringBehavior
{
    public Transform target;

    public float maxSpeed;
    public float slowingDistance;

    [ReadOnly] public Vector3 deltaTarget;
    [ReadOnly] public float targetDistance;
    [ReadOnly] public Vector3 targetDirection;
    [ReadOnly] public float rampedSpeed;
    [ReadOnly] public float clampedSpeed;

    void Update()
    {
        deltaTarget = target.position - transform.position;
        targetDistance = deltaTarget.magnitude;
        targetDirection = (targetDistance > 0) ? deltaTarget / targetDistance : Vector3.zero;

        rampedSpeed = maxSpeed * (targetDistance / slowingDistance);
        clampedSpeed = Mathf.Clamp(rampedSpeed, 0f, maxSpeed);

        desiredVelocity = targetDirection * clampedSpeed;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, target.position);
    }
}
