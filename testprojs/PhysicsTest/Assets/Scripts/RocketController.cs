using UnityEngine;

public class RocketController : MonoBehaviour
{
    public Rigidbody rb;

    public float inputThrust = 0f;
    public float inputSteer = 0f;
    public bool touchSupport = false;

    public float acceleration = 10f;
    public float angularVelocity = 6;

    public Vector3 gravity;
    public Vector3 airVelocity;
    public Vector3 airAngularVelocity;

    public float drag; // TODO Jonas: non-uniform drag
    public float angularDrag;

    void Start()
    {
        rb.maxAngularVelocity = 160;
    }

    void Update()
    {
        inputThrust = Input.GetAxis("Thrust");
        inputSteer = Input.GetAxis("Steer");

        if (touchSupport)
        {
            var left = Input.GetButton("TouchLeft");
            var right = Input.GetButton("TouchRight");
            inputThrust = (left && right) ? 1f : 0f;
            if (left && !right)
            {
                inputSteer = -1;
            }
            else if (right && !left)
            {
                inputSteer = 1;
            }
            else
            {
                inputSteer = 0;
            }
        }
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
            // gravity
            rb.AddForce(gravity, ForceMode.Acceleration);
        }

        {
            // drag
            var currentVelocity = rb.velocity;
            var targetVelocity = airVelocity;
            var deltaVelocity = targetVelocity - currentVelocity;
            var dragAcceleration = deltaVelocity * drag;
            rb.AddForce(dragAcceleration, ForceMode.Acceleration);
        }

        {
            // angular drag
            var currentAngularVelocity = rb.angularVelocity;
            var targetAngularVelocity = airAngularVelocity;
            var deltaAngularVelocity = targetAngularVelocity - currentAngularVelocity;
            var dragAngularAcceleration = deltaAngularVelocity * angularDrag;
            rb.AddTorque(dragAngularAcceleration, ForceMode.Acceleration);
        }

        {
            // align with 2D plane
            var currentUp = transform.up;
            var targetUp = -Vector3.forward;
            var angleAxis = Vector3.Cross(currentUp, targetUp);

            var deltaAngularVelocity = angleAxis;
            var angularAcceleration = deltaAngularVelocity * inverseDeltaTime; // TODO Jonas: use optimal control

            rb.AddTorque(angularAcceleration, ForceMode.Acceleration);
        }
    }
}
