using UnityEngine;
using System.Collections;

public class SeekBehavior : SteeringBehavior
{
    public float maxSpeed;

    public Transform target;

    public Vector3 toTarget;
    public Vector3 desiredVelocity;

    void Update()
    {
        toTarget = target.position - transform.position;
        desiredVelocity = Vector3.Normalize(toTarget) * maxSpeed;
        steeringDirection = desiredVelocity - rb.velocity;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, target.position);
    }
}
