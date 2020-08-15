using UnityEngine;

[CreateAssetMenu]
public sealed class RocketSpawnerObject : ScriptableObject
{
    [SerializeField] private GameObject placeholderPrefab;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private Vector3 spawnRotation;
    [SerializeField] private float respawnDelay = 5f;

    public void OnDeath()
    {
        var placeholder = Object.Instantiate(placeholderPrefab, spawnPosition, Quaternion.Euler(spawnRotation));
        Object.Destroy(placeholder, respawnDelay);
    }
}
