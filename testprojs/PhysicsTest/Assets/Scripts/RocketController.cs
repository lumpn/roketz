using UnityEngine;

public class RocketController : MonoBehaviour
{
    public Rigidbody rb;

    public float thrustScalar = 10f;
    public float steerScalar = 1f;

    public float inputThrust = 0f;
    public float inputSteer = 0f;

    public Vector3 thrust;
    public Vector3 steer;

    public float maxAngularVelocity = 10;
    public float maxAngularAcceleration = 10;

    public Vector3 targetAngularVelocity;
    public Vector3 targetAngularAcceleration;

    void Start ()
    {
        rb.maxAngularVelocity = 160;
    }

    void Update ()
    {
        inputThrust = Input.GetAxis ("Thrust");
        inputSteer = Input.GetAxis ("Steer");
    }

    void FixedUpdate ()
    {
        thrust = transform.forward * (inputThrust * thrustScalar);
        rb.AddForce (thrust * Time.fixedDeltaTime);

        var steering = Mathf.Clamp (inputSteer * steerScalar, -maxAngularVelocity, maxAngularVelocity);
        targetAngularVelocity = Vector3.forward * steering;

        var deltaAngularVelocity = targetAngularVelocity - rb.angularVelocity;
        targetAngularAcceleration = deltaAngularVelocity / Time.fixedDeltaTime;
        var appliedAngularAcceleration = Vector3.ClampMagnitude (targetAngularAcceleration, maxAngularAcceleration);

        rb.AddTorque (appliedAngularAcceleration, ForceMode.Acceleration);
    }
}
