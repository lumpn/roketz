using UnityEngine;

public sealed class FuelPoint : MonoBehaviour
{
    [SerializeField] private RocketHull hull;
    [SerializeField] private float hitpointRate = 50;

    private bool fueling;

    void Update()
    {
        if (!fueling) return;

        hull.AddHitpoints(hitpointRate * Time.deltaTime);
    }

    public void StartFueling()
    {
        fueling = true;
    }

    public void StopFueling()
    {
        fueling = false;
    }
}
