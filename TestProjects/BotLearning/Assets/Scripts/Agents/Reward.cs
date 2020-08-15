using UnityEngine;

public sealed class Reward : MonoBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        var agent = GetAgent(coll);
        if (!agent) return;

        // reward agent
        agent.AddReward(1);

        // destroy reward
        Object.Destroy(gameObject);
    }

    private static RocketAgent GetAgent(Collider collider)
    {
        var rb = collider.attachedRigidbody;
        if (!rb) return null;

        // reward agent
        return rb.GetComponent<RocketAgent>();
    }
}
