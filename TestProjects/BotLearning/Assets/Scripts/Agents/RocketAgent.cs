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
    }

    public void OnValueChanged(FloatObject obj, float oldValue, float newValue)
    {
        var delta = newValue - oldValue;
        AddReward(delta);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        var thrust = vectorAction[0];
        var steer = vectorAction[1];
        controller.SetInput(thrust, steer);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // TODO Jonas: collect observations
    }
}
