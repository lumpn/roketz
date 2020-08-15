using UnityEngine;
using System.Collections;

public class SeekBehavior : SteeringBehavior
{
    public Transform target;

    public float maxSpeed;

    [ReadOnly] public Vector3 deltaTarget;
    [ReadOnly] public Vector3 targetDirection;

    void Update()
    {
        deltaTarget = target.position - transform.position;
        targetDirection = deltaTarget.normalized;

        desiredVelocity = targetDirection * maxSpeed;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, target.position);
    }
}
