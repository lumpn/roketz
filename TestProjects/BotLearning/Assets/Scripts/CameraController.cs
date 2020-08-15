using UnityEngine;

public sealed class CameraController : MonoBehaviour
{
    [SerializeField] private RigidbodyObject target;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float slowingDistance = 10;
    [SerializeField] private float maxVelocity = 100;
    [SerializeField] private float maxAcceleration = 1000;

    void FixedUpdate()
    {
        var targetRb = target.Rigidbody;
        if (!targetRb) return;

        var targetPosition = targetRb.position;
        var currentPosition = rb.position;
        var deltaTarget = targetPosition - currentPosition;
        var targetDistance = deltaTarget.magnitude;
        var targetDirection = (targetDistance > 0) ? deltaTarget / targetDistance : Vector3.zero;

        var rampedVelocity = maxVelocity * (targetDistance / slowingDistance);
        var clampedVelocity = Mathf.Clamp(rampedVelocity, 0f, maxVelocity);

        var desiredVelocity = targetDirection * clampedVelocity;
        var currentVelocity = rb.velocity;
        var deltaVelocity = desiredVelocity - currentVelocity;

        var desiredAcceleration = deltaVelocity / Time.fixedDeltaTime;
        var appliedAcceleration = Vector3.ClampMagnitude(desiredAcceleration, maxAcceleration);

        rb.AddForce(appliedAcceleration, ForceMode.Acceleration);
    }
}
