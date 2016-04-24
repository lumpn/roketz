using UnityEngine;

public static class GameObjectUtils
{
    public static void Destroy (Object obj)
    {
        if (Application.isPlaying) {
            Object.Destroy (obj);
        } else {
            Object.DestroyImmediate (obj);
        }
    }
}
