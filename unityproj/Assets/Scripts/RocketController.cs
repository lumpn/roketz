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

    void Update ()
    {
        inputThrust = Input.GetAxis ("Thrust");
        inputSteer = Input.GetAxis ("Steer");
    }

    void FixedUpdate ()
    {
        var fwd = transform.forward;
        var up = transform.up;
        thrust = fwd * (inputThrust * thrustScalar);
        steer = up * (inputSteer * steerScalar);

        rb.AddForce (thrust * Time.fixedDeltaTime);
        rb.AddTorque (steer * Time.fixedDeltaTime);
    }
}
