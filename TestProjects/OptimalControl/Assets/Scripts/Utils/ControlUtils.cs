using UnityEngine;

/// assuming Unity physics
/// p1 = p0 + v1 * t
/// v1 = v0 + a0 * t
public static class ControlUtils
{
    public static float CalcAcceleration(float currentPosition, float targetPosition, float currentVelocity, float deltaTime)
    {
        var deltaPosition = targetPosition - currentPosition;
        return (deltaPosition - currentVelocity * deltaTime) / (deltaTime * deltaTime);
    }

    public static float CalcAcceleration(float currentPosition, float targetPosition, float currentVelocity, float maxAcceleration, float deltaTime)
    {
        var deltaPosition = targetPosition - currentPosition;
        var fullAcceleration = (deltaPosition - currentVelocity * deltaTime) / (deltaTime * deltaTime);

        var acceleration = Mathf.Clamp(fullAcceleration, -maxAcceleration, maxAcceleration);
        var nextVelocity = currentVelocity + acceleration * deltaTime;
        var nextPosition = currentPosition + nextVelocity * deltaTime;

        // we might be overshooting
        var nextAcceleration = CalcAcceleration(nextPosition, targetPosition, nextVelocity, deltaTime);
        if (nextAcceleration * acceleration < 0) // sign changed
        {
            if (nextAcceleration < -maxAcceleration || nextAcceleration > maxAcceleration)
            {
                // overshooting. need to slow down
                return 0; // TODO Jonas: calculate
            }
        }

        return acceleration;
    }

    public static float Damp(float currentPosition, float targetPosition, ref float currentVelocity, float deltaTime)
    {
        var acceleration = CalcAcceleration(currentPosition, targetPosition, currentVelocity, deltaTime);
        var nextVelocity = currentVelocity + acceleration * deltaTime;
        var nextPosition = currentPosition + nextVelocity * deltaTime;
        currentVelocity = nextVelocity;
        return nextPosition;
    }
}
