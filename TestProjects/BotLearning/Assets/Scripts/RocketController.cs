using UnityEngine;

public sealed class RocketController : MonoBehaviour
{
    private const float springConstant = 20;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private float acceleration = 80;
    [SerializeField] private float angularVelocity = 5;

    private float inputThrust = 0;
    private float inputSteer = 0;

    public void SetInput(float thrust, float steer)
    {
        inputThrust = Mathf.Clamp01(thrust);
        inputSteer = Mathf.Clamp(steer, -1, 1);
    }

    void Start()
    {
        rb.maxAngularVelocity = 16;
    }

    void FixedUpdate()
    {
        var inverseDeltaTime = 1f / Time.fixedDeltaTime;

        {
            // thrust
            var targetAcceleration = Mathf.Clamp01(inputThrust) * acceleration;
            rb.AddRelativeForce(Vector3.forward * targetAcceleration, ForceMode.Acceleration);
        }

        {
            // steer
            var currentAngularVelocity = -rb.angularVelocity.z;// TODO Jonas: technically we need angVel.y in local space here
            var targetAngularVelocity = Mathf.Clamp(inputSteer, -1f, 1f) * angularVelocity;
            var deltaAngularVelocity = targetAngularVelocity - currentAngularVelocity;

            // angular acceleration
            var targetAngularAcceleration = deltaAngularVelocity * inverseDeltaTime; // TODO Jonas: use optimal control
            rb.AddRelativeTorque(Vector3.up * targetAngularAcceleration, ForceMode.Acceleration);
        }

        {
            // stay on 2D plane
            var deltaZ = -rb.position.z;
            var springAcceleration = deltaZ * springConstant; // TODO Jonas: use optimal control
            rb.AddForce(Vector3.forward * springAcceleration, ForceMode.Acceleration);
        }

        {
            // align with 2D plane
            var currentUp = transform.up;
            var targetUp = -Vector3.forward;
            var angleAxis = Vector3.Cross(currentUp, targetUp);

            var deltaAngle = angleAxis;
            var angularAcceleration = deltaAngle * inverseDeltaTime; // TODO Jonas: use optimal control

            rb.AddTorque(angularAcceleration, ForceMode.Acceleration);
        }
    }
}
