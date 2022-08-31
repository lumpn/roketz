using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public sealed class BalancingAgent : Agent
{
    [SerializeField] private Transform initialBallPosition;
    [SerializeField] private Rigidbody ball;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float maxRotationDelta;

    public override void OnEpisodeBegin()
    {
        ball.position = initialBallPosition.position;
        ball.velocity = Random.insideUnitSphere * maxVelocity;
        transform.localRotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(ball.position - transform.position);
        sensor.AddObservation(ball.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var control = new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]);
        var desiredAngles = control * 30;
        var currentAngles = transform.localRotation.eulerAngles;
        var angleX = Mathf.MoveTowardsAngle(currentAngles.x, desiredAngles.x, maxRotationDelta);
        var angleZ = Mathf.MoveTowardsAngle(currentAngles.z, desiredAngles.z, maxRotationDelta);

        transform.localRotation = Quaternion.Euler(angleX, 0, angleZ);

        AddReward(0.1f);

        if (ball.position.y < 0)
        {
            EndEpisode();
        }
    }

    protected void FixedUpdate()
    {
        RequestDecision();
    }
}
