using UnityEngine;

public sealed class PlayerInput : MonoBehaviour
{
    [SerializeField] private RocketController controller;

    void Update()
    {
        if (!controller) return;

        var thrust = Input.GetAxis("Thrust");
        var steer = Input.GetAxis("Steer");
        controller.SetInput(thrust, steer);
    }
}
