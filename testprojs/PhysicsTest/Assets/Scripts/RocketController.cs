using UnityEngine;

public class RocketController : MonoBehaviour
{
    public Rigidbody rb;

    public float thrustScalar = 10f;

    public float inputThrust = 0f;
    public float inputSteer = 0f;

    public Vector3 thrust;
    public Vector3 steer;

    public float maxVelocity = 100;
    public float maxAcceleration = 10f;
    public float maxDeceleration = 10f;

    public float maxAngularVelocity = 6;
    public float maxAngularAcceleration = 60;

    public Vector3 gravity;
    public Vector3 airVelocity;
    public Vector3 airAngularVelocity;
    public float dragScalar;
    public float angularDragScalar;

    public Vector3 aboveVelocity;
    public Vector3 aboveAcceleration;

    void Start()
    {
        rb.maxAngularVelocity = 160;
    }

    void Update()
    {
        inputThrust = Input.GetAxis("Thrust");
        inputSteer = Input.GetAxis("Steer");
    }

    void FixedUpdate()
    {
        {
            // gravity
            rb.AddForce(gravity, ForceMode.Acceleration);
        }

        {
            // drag
            var currentVelocity = rb.velocity;
            var targetVelocity = airVelocity;
            var deltaVelocity = targetVelocity - currentVelocity;
            var dragAcceleration = (deltaVelocity / Time.fixedDeltaTime) * dragScalar;
            rb.AddForce(dragAcceleration, ForceMode.Acceleration);
        }

        {
            // max velocity
            var currentVelocity = rb.velocity;
            var clampedVelocity = Vector3.ClampMagnitude(currentVelocity, maxVelocity);
            aboveVelocity = currentVelocity - clampedVelocity;
            aboveAcceleration = aboveVelocity / Time.fixedDeltaTime;
            rb.AddForce(aboveAcceleration, ForceMode.Acceleration);
        }

        {
            // angular drag
            var currentAngularVelocity = rb.angularVelocity;
            var targetAngularVelocity = airAngularVelocity;
            var deltaAngularVelocity = targetAngularVelocity - currentAngularVelocity;
            var dragAngularAcceleration = (deltaAngularVelocity / Time.fixedDeltaTime) * angularDragScalar;
            rb.AddTorque(dragAngularAcceleration, ForceMode.Acceleration);
        }

        {
            // thrust
            var targetAcceleration = Mathf.Clamp01(inputThrust) * maxAcceleration;
            rb.AddForce(transform.forward * targetAcceleration, ForceMode.Acceleration);
        }

        {
            // steer
            var targetAngularVelocity = Mathf.Clamp(-inputSteer, -1f, 1f) * maxAngularVelocity + airAngularVelocity.z;
            var currentAngularVelocity = rb.angularVelocity.z;
            var deltaAngularVelocity = targetAngularVelocity - currentAngularVelocity;

            // angular acceleration
            var targetAngularAcceleration = deltaAngularVelocity / Time.fixedDeltaTime;
            var appliedAngularAcceleration = Mathf.Clamp(targetAngularAcceleration, -maxAngularAcceleration, maxAngularAcceleration);
            rb.AddTorque(Vector3.forward * appliedAngularAcceleration, ForceMode.Acceleration);
        }
    }
}
