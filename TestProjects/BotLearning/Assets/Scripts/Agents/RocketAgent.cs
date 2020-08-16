using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public sealed class RocketAgent : Agent, IFloatListener
{
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
    }

    public void OnValueChanged(FloatObject obj, float oldValue, float newValue)
    {
        var delta = newValue - oldValue;
        AddReward(delta);
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");
        // TODO Jonas: reset
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        var thrust = vectorAction[0];
        var steer = vectorAction[1];
        Debug.LogFormat("OnActionReceived thrust {0}, steer {1}", thrust, steer);
        controller.SetInput(thrust, steer);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // TODO Jonas: collect observations
        sensor.AddObservation(transform.position);
    }

    public override void Heuristic(float[] actionsOut)
    {
        var thrust = Input.GetAxis("Thrust");
        var steer = Input.GetAxis("Steer");
        actionsOut[0] = thrust;
        actionsOut[1] = steer;
        Debug.LogFormat("Heuristic thrust {0}, steer {1}", thrust, steer);
    }
}
