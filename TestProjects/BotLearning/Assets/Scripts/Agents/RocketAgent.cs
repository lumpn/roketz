using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public sealed class RocketAgent : Agent, IFloatListener
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private RocketController controller;
    [SerializeField] private FloatObject hitpoints;

    void Start()
    {
        hitpoints.AddListener(this);
    }

    void OnDestroy()
    {
        hitpoints.RemoveListener(this);

        // game over
        EndEpisode();

        // TODO Jonas: respawn agent
    }

    public void OnValueChanged(FloatObject obj, float oldValue, float newValue)
    {
        var delta = newValue - oldValue;
        AddReward(delta);
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");

        // reset hitpoints
        hitpoints.RemoveListener(this);
        hitpoints.Reset();
        hitpoints.AddListener(this);

        // reset position
        rb.position = Vector3.zero;
        rb.rotation = Quaternion.Euler(-90, 0, 0);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // TODO Jonas: reset rewards
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        var thrust = vectorAction[0];
        var steer = vectorAction[1];
        controller.SetInput(thrust, steer);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        // TODO Jonas: collect observations
    }

    public override void Heuristic(float[] actionsOut)
    {
        var thrust = Input.GetAxis("Thrust");
        var steer = Input.GetAxis("Steer");
        actionsOut[0] = thrust;
        actionsOut[1] = steer;
    }
}
