using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;

    public float slowingDistance = 10;
    public float maxVelocity = 100;
    public float maxAcceleration = 1000;

    void FixedUpdate()
    {
        var targetPosition = target.position;
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
