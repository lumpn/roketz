using UnityEngine;
using System.Collections;

public class Manipulator : MonoBehaviour
{
    public Rigidbody rb;

    public Vector3 targetAngularVelocity;

    [Range(0, 700)] // Enough to fully accelerate from +7 to -7 rad/s in one frame (50ms).
    public float maxAngularAcceleration;
    public bool controlAngularVelocity;

    [Range(0, 7)] // Unity does not support angular velocity above 7 radians per second.
    public float maxAngularVelocity;
    public Vector3 targetOrientation;
    public bool controlOrientation;

    public Vector3 appliedAngularVelocity;


    void FixedUpdate()
    {
        if (controlOrientation)
        {
            var curFwd = transform.forward;
            var tarFwd = targetOrientation;
            var rotAxis = Vector3.Cross(curFwd, tarFwd);
            var deltaAngle = Vector3.Angle(curFwd, tarFwd) * Mathf.Deg2Rad;
            var fullOmega = deltaAngle / Time.deltaTime;
            var clampOmega = Mathf.Clamp(fullOmega, 0f, maxAngularVelocity);

            targetAngularVelocity = Vector3.Normalize(rotAxis) * clampOmega;
        }

        if (controlAngularVelocity)
        {
            var curAngVel = rb.angularVelocity;
            var stopAngAcc = -curAngVel / Time.fixedDeltaTime;
            var tarAngVel = targetAngularVelocity;
            var accAngAcc = tarAngVel / Time.fixedDeltaTime;
            var angAcc = stopAngAcc + accAngAcc;

            appliedAngularVelocity = Vector3.ClampMagnitude(angAcc, maxAngularAcceleration);

            rb.AddTorque(appliedAngularVelocity, ForceMode.Acceleration);
        }
    }
}
