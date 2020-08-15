using UnityEngine;

public sealed class RocketHull : MonoBehaviour
{
    [SerializeField] private RocketSpawnerObject spawner;
    [SerializeField] private Component[] destroyOnDeath;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float hitpoints = 100;
    [SerializeField] private float damageFactor = 1;
    [SerializeField] private float explosionStrength = 10;
    [SerializeField] private float despawnDelay = 5f;

    void OnCollisionEnter(Collision collision)
    {
        var impulse = collision.impulse;
        hitpoints -= impulse.magnitude * damageFactor;

        // death?
        if (hitpoints <= 0)
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
        if (!UnityEditor.EditorApplication.isPlaying) return;
        Debug.Log("RocketHull OnDestroy");

        // inform spawner
        spawner.OnDeath();
    }
}
