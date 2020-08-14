using UnityEngine;
using System.Collections;

public class Manipulator : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    /// NOTE Jonas: Twice maximum angular velocity times 50 is enough to fully accelerate
    /// from -maxAngVel to +maxAngVel in one frame (50ms). 700 would be enough to cover
    /// the default maxAngVel of 7 radians per second.
    [Range(0, 16000)]
    [SerializeField] private float maxAngularAcceleration;
    [SerializeField] private bool controlAngularVelocity;

    /// NOTE Jonas: Unity does not support angular velocity above 7 radians per second by default.
    /// Use rigibody.maxAngularVelocity to change the limit. The maximum sensible angular velocity is 160.
    /// It allows for rotations to any orientation within one frame (50ms).
    [Range(0, 160)]
    [SerializeField] private float maxAngularVelocity;
    [SerializeField] private Vector3 targetOrientation = Vector3.forward;
    [SerializeField] private bool controlOrientation;

    // debug
    private Vector3 targetAngularVelocity;
    private Vector3 appliedAngularVelocity;

    public Rigidbody Rigidbody { get { return rb; } }
    public Vector3 TargetAngularVelocity { get { return targetAngularVelocity; } }
    public Vector3 AppliedAngularVelocity { get { return appliedAngularVelocity; } }
    public Vector3 TargetOrientation { set { targetOrientation = value; } }

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
            var angAcc = stopAngAcc + accAngAcc; // is this correct?

            appliedAngularVelocity = Vector3.ClampMagnitude(angAcc, maxAngularAcceleration);

            rb.AddTorque(appliedAngularVelocity, ForceMode.Acceleration);
        }
    }
}
