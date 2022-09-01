using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public sealed class BallisticsAgent : Agent
{
    private static readonly float sin45 = 0.5f * Mathf.Sqrt(2);
    private static readonly float cos45 = sin45;

    [SerializeField] private Transform target;
    [SerializeField] private Bounds range;
    [SerializeField] private float radius;
    [SerializeField] private float gravity;
    [SerializeField] private bool drawTrajectory;
    [SerializeField] private int numTrajectorySegments;
    [SerializeField] private Color trajectoryColor;
    [SerializeField] private float trajectoryDuration;

    public override void OnEpisodeBegin()
    {
        var posX = Random.Range(range.min.x, range.max.x);
        var posY = Random.Range(range.min.y, range.max.y);
        var posZ = Random.Range(range.min.z, range.max.z);
        var pos = new Vector3(posX, posY, posZ);
        transform.localPosition = pos;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var toTarget = target.position - transform.position;
        var normalized = toTarget / radius;
        sensor.AddObservation(normalized);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var rotationValue = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f) * 180;
        var velocityValue = Mathf.Clamp(actions.ContinuousActions[1], 0f, 1f) * 25;

        rotationValue = 0;
        var rotation = Quaternion.Euler(0, rotationValue, 0);
        var velocity = new Vector3(0, velocityValue * sin45, velocityValue * cos45);

        if (drawTrajectory)
        {
            DrawTrajectory(rotation, velocity);
        }

        var flightDuration = 2 * velocity.y / gravity;
        var p0 = transform.position;
        var acceleration = new Vector3(0, -gravity, 0);
        var impactPosition = p0 + velocity * flightDuration + 0.5f * acceleration * flightDuration * flightDuration;

        var targetPosition = target.position;
        var delta = impactPosition - targetPosition;

        SetReward(-delta.magnitude);
        EndEpisode();
    }

    protected void Update()
    {
        RequestDecision();
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(range.center, range.size);
        Gizmos.DrawWireSphere(Vector3.zero, radius);
    }

    public void DrawTrajectory(Quaternion rotation, Vector3 velocity)
    {
        DrawTrajectory(rotation * velocity);
    }

    private void DrawTrajectory(Vector3 velocity)
    {
        var flightDuration = 2 * velocity.y / gravity;
        var segmentDuration = flightDuration / numTrajectorySegments;

        var p0 = transform.position;
        var v0 = velocity;
        var a = new Vector3(0, -gravity, 0);

        var lastP = p0;
        for (int i = 1; i <= numTrajectorySegments; i++)
        {
            var t = segmentDuration * i;
            var p = p0 + v0 * t + 0.5f * a * t * t;

            Debug.DrawLine(lastP, p, trajectoryColor, trajectoryDuration);
            lastP = p;
        }
    }
}
