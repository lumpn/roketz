using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public Rigidbody rb;

    public float maxVelocity = 100;
    public float maxAcceleration = 1000;

    public float stickyScalar = 0.1f;

    [ReadOnly] public Vector3 desiredVelocity;
    [ReadOnly] public Vector3 targetVelocity;
    [ReadOnly] public Vector3 desiredAcceleration;
    [ReadOnly] public Vector3 targetAcceleration;

    void FixedUpdate()
    {
        var targetPosition = target.position + offset;
        var currentPosition = rb.position;
        var deltaPosition = targetPosition - currentPosition;

         desiredVelocity = (deltaPosition / Time.fixedDeltaTime) * stickyScalar;
         targetVelocity = Vector3.ClampMagnitude(desiredVelocity, maxVelocity);
        var currentVelocity = rb.velocity;
        var deltaVelocity = targetVelocity - currentVelocity;

         desiredAcceleration = deltaVelocity / Time.fixedDeltaTime;
         targetAcceleration = Vector3.ClampMagnitude(desiredAcceleration, maxAcceleration);

        rb.AddForce(targetAcceleration, ForceMode.Acceleration);
    }
}
