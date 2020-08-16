using UnityEngine;

public sealed class Reward : MonoBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        Debug.LogFormat("Reward hit by {0}", coll.name);
        var agent = GetAgent(coll);
        if (!agent) return;

        // reward agent
        Debug.LogFormat("Rewarding agent {0}", agent.name);
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
