using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MyAgent : Agent
{
    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");
        transform.localPosition = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        transform.localPosition = new Vector3(vectorAction[0], vectorAction[1], 0);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }
}
