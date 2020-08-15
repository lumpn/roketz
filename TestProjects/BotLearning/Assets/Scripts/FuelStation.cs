using UnityEngine;

public sealed class FuelStation : MonoBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        var fuelPoint = coll.GetComponent<FuelPoint>();
        if (!fuelPoint) return;

        fuelPoint.StartFueling();
    }

    void OnTriggerExit(Collider coll)
    {
        var fuelPoint = coll.GetComponent<FuelPoint>();
        if (!fuelPoint) return;

        fuelPoint.StopFueling();
    }
}
