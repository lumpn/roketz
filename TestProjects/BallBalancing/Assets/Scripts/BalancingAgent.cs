using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BalancingAgent : Agent
{
    [SerializeField] private Transform initialBallPosition;
    [SerializeField] private Rigidbody ball;

    public override void OnEpisodeBegin()
    {
        ball.position = initialBallPosition.position;
        ball.velocity = new Vector3(Random.Range(-2, 2), 0, 0);
        transform.localRotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(ball.position.x - transform.position.x);
        sensor.AddObservation(ball.velocity.x);

        //Debug.LogFormat("Sending angle {0}, pos {1}, vel {2}", transform.localRotation.eulerAngles.z, ball.position.x - transform.position.x, ball.velocity.x);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var desiredAngleZ = 90 * actions.ContinuousActions[0];
        var oldAngleZ = transform.localRotation.eulerAngles.z;
        var newAngleZ = Mathf.MoveTowardsAngle(oldAngleZ, desiredAngleZ, 1f);

        transform.localRotation = Quaternion.Euler(0, 0, newAngleZ);

        //Debug.LogFormat("Received angle {0}", angleZ);
        AddReward(0.1f);

        if (ball.position.y < 0)
        {
            EndEpisode();
        }
    }
}
