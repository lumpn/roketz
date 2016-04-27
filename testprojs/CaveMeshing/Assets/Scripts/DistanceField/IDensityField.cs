using UnityEngine;

/// Signed density field.
/// Positive density means "inside".
/// Negative density means "outside".
public interface IDensityField
{
    /// Evaluates the density at the specified position.
    /// <returns>Positive values are inside, negative outside.</returns>
    float Evaluate(Vector3 position);
}
