using UnityEngine;
using System.Collections;

public class Manipulator : MonoBehaviour
{
    public Rigidbody rb;

    public Vector3 targetAngularVelocity;

    /// NOTE Jonas: Twice maximum angular velocity times 50 is enough to fully accelerate
    /// from -maxAngVel to +maxAngVel in one frame (50ms). 700 would be enough to cover
    /// the default maxAngVel of 7 radians per second.
    [Range(0, 16000)] // E
    public float maxAngularAcceleration;
    public bool controlAngularVelocity;

    /// NOTE Jonas: Unity does not support angular velocity above 7 radians per second by default.
    /// Use rigibody.maxAngularVelocity to change the limit. The maximum sensible angular velocity is 160.
    /// It allows for rotations to any orientation within one frame (50ms).
    [Range(0, 160)]
    public float maxAngularVelocity;
    public Vector3 targetOrientation = Vector3.forward;
    public bool controlOrientation;

    public Vector3 appliedAngularVelocity;

    void Start()
    {
        rb.maxAngularVelocity = 160;
    }

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
