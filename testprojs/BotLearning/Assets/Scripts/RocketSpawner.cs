using UnityEngine;

public sealed class RocketSpawner : MonoBehaviour
{
    [SerializeField] private RigidbodyObject rigidbodyObject;
    [SerializeField] private Rigidbody rocketPrefab;

    void OnDestroy()
    {
        if (!Application.isPlaying) return;

        Debug.Log("RocketSpawner OnDestroy");
        var rocket = Object.Instantiate(rocketPrefab, transform.position, transform.rotation);
        rigidbodyObject.Rigidbody = rocket;
    }
}
