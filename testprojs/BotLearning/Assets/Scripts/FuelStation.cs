using UnityEngine;

public sealed class FuelStation : MonoBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        Debug.LogFormat("OnTriggerEnter {0}", coll.name);
        var fuelPoint = coll.GetComponent<FuelPoint>();
        if (!fuelPoint) return;

        fuelPoint.StartFueling();
    }

    void OnTriggerExit(Collider coll)
    {
        Debug.LogFormat("OnTriggerExit {0}", coll.name);
        var fuelPoint = coll.GetComponent<FuelPoint>();
        if (!fuelPoint) return;

        fuelPoint.StopFueling();
    }
}
