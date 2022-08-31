using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BalancingAgent : Agent
{
    [SerializeField] private Transform initialBallPosition;
    [SerializeField] private Rigidbody ball;
    [SerializeField] private float maxRotationDelta;

    public override void OnEpisodeBegin()
    {
        ball.position = initialBallPosition.position;
        ball.velocity = Vector3.zero; //Random.insideUnitSphere * 0.1f;
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
        var desiredAngles = control * 90;
        var currentAngles = transform.localRotation.eulerAngles;
        var angles = Vector3.MoveTowards(currentAngles, desiredAngles, maxRotationDelta);

        transform.localRotation = Quaternion.Euler(angles);

        AddReward(0.1f);

        if (ball.position.y < 0)
        {
            EndEpisode();
        }
    }
}
