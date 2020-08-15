using UnityEngine;

public sealed class RocketHull : MonoBehaviour
{
    [SerializeField] private RocketSpawnerObject spawner;
    [SerializeField] private Component[] destroyOnDeath;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private FloatObject hitpoints;

    [SerializeField] private float damageFactor = 1;
    [SerializeField] private float explosionStrength = 10;
    [SerializeField] private float despawnDelay = 5f;

    public void AddHitpoints(float value)
    {
        hitpoints.Value += value;
    }

    void Start()
    {
        hitpoints.Reset();
    }

    void OnCollisionEnter(Collision collision)
    {
        var impulse = collision.impulse;
        hitpoints.Value -= impulse.magnitude * damageFactor;

        // death?
        if (hitpoints.Value <= 0)
        {
            // destroy components
            foreach (var component in destroyOnDeath)
            {
                Object.Destroy(component);
            }

            // apply explosion
            rb.AddForceAtPosition(Random.onUnitSphere * explosionStrength, Random.onUnitSphere, ForceMode.VelocityChange);

            // schedule despawn
            Object.Destroy(gameObject, despawnDelay);

            // self destruct
            Object.Destroy(this);
        }
    }

    void OnDestroy()
    {
        // inform spawner
        spawner.OnDeath();
    }
}
