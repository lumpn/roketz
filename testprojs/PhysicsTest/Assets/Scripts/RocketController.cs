using UnityEngine;

public class RocketController : MonoBehaviour
{
    public Rigidbody rb;

    public float inputThrust = 0f;
    public float inputSteer = 0f;
    public bool touchSupport = false;

    public float acceleration = 10f;

    public float angularVelocity = 6;
    public float maxAngularAcceleration = 60;

    public Vector3 gravity;
    public Vector3 airVelocity;
    public Vector3 airAngularVelocity;

    public float dragScalar; // TODO Jonas: non-uniform drag
    public float angularDragScalar;
    public float alignmentScalar;

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
        var currentVelocity = rb.velocity - airVelocity;
        var currentAngularVelocity = rb.angularVelocity - airAngularVelocity;
        var inverseDeltaTime = 1f / Time.fixedDeltaTime;

        {
            // thrust
            var targetAcceleration = Mathf.Clamp01(inputThrust) * acceleration;
            rb.AddRelativeForce(Vector3.forward * targetAcceleration, ForceMode.Acceleration);
        }

        {
            // steer
            var targetAngularVelocity = Mathf.Clamp(-inputSteer, -1f, 1f) * angularVelocity;
            var deltaAngularVelocity = targetAngularVelocity - currentAngularVelocity.z;

            // angular acceleration
            var targetAngularAcceleration = deltaAngularVelocity * inverseDeltaTime; // TODO Jonas: use optimal control
            var appliedAngularAcceleration = Mathf.Clamp(targetAngularAcceleration, -maxAngularAcceleration, maxAngularAcceleration);
            rb.AddRelativeTorque(Vector3.up * appliedAngularAcceleration, ForceMode.Acceleration);
        }

        {
            // gravity
            rb.AddForce(gravity, ForceMode.Acceleration);
        }

        {
            // drag
            var targetVelocity = Vector3.zero;
            var deltaVelocity = targetVelocity - currentVelocity;
            var dragAcceleration = deltaVelocity * (inverseDeltaTime * dragScalar);
            rb.AddForce(dragAcceleration, ForceMode.Acceleration);
        }

        {
            // angular drag
            var targetAngularVelocity = Vector3.zero;
            var deltaAngularVelocity = targetAngularVelocity - currentAngularVelocity;
            var dragAngularAcceleration = deltaAngularVelocity * (inverseDeltaTime * angularDragScalar);
            rb.AddTorque(dragAngularAcceleration, ForceMode.Acceleration);
        }

        //{
        //    // align with 2D plane
        //    var currentUp = transform.up;
        //    var targetUp = -Vector3.forward;
        //    var axis = Vector3.Cross(currentUp, targetUp);
        //    var angularAcceleration = axis * alignmentScalar;
        //    rb.AddTorque(angularAcceleration, ForceMode.Acceleration);
        //}
    }
}
