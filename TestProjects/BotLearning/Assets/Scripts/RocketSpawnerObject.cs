using UnityEngine;

[CreateAssetMenu]
public sealed class RocketSpawnerObject : ScriptableObject
{
    [SerializeField] private RocketSpawner spawnerPrefab;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private Vector3 spawnRotation;
    [SerializeField] private float respawnDelay = 5f;

    public void OnDeath()
    {
        var spawner = Object.Instantiate(spawnerPrefab, spawnPosition, Quaternion.Euler(spawnRotation));
        Object.Destroy(spawner, respawnDelay);
    }
}
