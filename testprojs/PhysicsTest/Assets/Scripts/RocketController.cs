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

    public float currentVelocity;
    public float targetAcceleration;
    public float appliedAcceleration;

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
        targetAcceleration = Mathf.Clamp01(inputThrust) * maxAcceleration;
        appliedAcceleration = Mathf.Clamp(targetAcceleration, -maxDeceleration, maxAcceleration);
        rb.AddForce(transform.forward * appliedAcceleration, ForceMode.Acceleration);


        var targetAngularVelocity = Mathf.Clamp(-inputSteer, -1f, 1f) * maxAngularVelocity;
        var currentAngularVelocity = rb.angularVelocity.z;
        var deltaAngularVelocity = targetAngularVelocity - currentAngularVelocity;

        var targetAngularAcceleration = deltaAngularVelocity / Time.fixedDeltaTime;
        var appliedAngularAcceleration = Mathf.Clamp(targetAngularAcceleration, -maxAngularAcceleration, maxAngularAcceleration);

        rb.AddTorque(Vector3.forward * appliedAngularAcceleration, ForceMode.Acceleration);
    }
}
