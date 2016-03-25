using UnityEngine;
using System.Collections;

public class Manipulator : MonoBehaviour
{
    public Rigidbody rb;

    public Vector3 targetAngularVelocity;
    public float maxAngularAcceleration;
    public bool controlAngularVelocity;

    public Vector3 appliedAngularVelocity;


    void FixedUpdate()
    {
        if (controlAngularVelocity)
        {
            var curAngVel = rb.angularVelocity;
            var stopAngAcc = -curAngVel / Time.fixedDeltaTime;
            var tarAngVel = targetAngularVelocity;
            var accAngAcc = tarAngVel / Time.fixedDeltaTime;
            var angAcc = stopAngAcc + accAngAcc;

            appliedAngularVelocity = Vector3.ClampMagnitude(angAcc, maxAngularAcceleration);
//            // clamp magnitude
//            var tmp = angAcc;
//            var mag = tmp.magnitude;
//            if (mag > maxAngularAcceleration)
//            {
//                tmp *= maxAngularAcceleration / mag;
//            }
//            appliedAngularVelocity = tmp;

            rb.AddTorque(appliedAngularVelocity, ForceMode.Acceleration);
        }
    }
}
