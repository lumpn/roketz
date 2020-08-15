using UnityEngine;

public sealed class DestroyOnStart : MonoBehaviour
{
    void Start()
    {
        Object.Destroy(gameObject);
    }
}
