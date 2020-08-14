using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Manipulator))]
public class ManipulatorEditor : Editor
{
    private Vector3 velocity;
    private Vector3 acceleration;

    private Vector3 angularVelocity;
    private Vector3 angularAcceleration;

    private Transform aimTarget;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var manipulator = (Manipulator)target;
        var rb = manipulator.Rigidbody;

        GUILayout.BeginHorizontal();
        velocity = EditorGUILayout.Vector3Field("Velocity", velocity);
        if (GUILayout.Button("Set", GUILayout.Width(80)))
        {
            rb.velocity = velocity;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        acceleration = EditorGUILayout.Vector3Field("Acceleration", acceleration);
        if (GUILayout.Button("Apply", GUILayout.Width(80)))
        {
            rb.AddForce(acceleration, ForceMode.Acceleration);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        angularVelocity = EditorGUILayout.Vector3Field("Angular velocity", angularVelocity);
        if (GUILayout.Button("Set", GUILayout.Width(80)))
        {
            rb.angularVelocity = angularVelocity;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        angularAcceleration = EditorGUILayout.Vector3Field("Angular acceleration", angularAcceleration);
        if (GUILayout.Button("Apply", GUILayout.Width(80)))
        {
            rb.AddTorque(angularAcceleration, ForceMode.Acceleration);
        }
        GUILayout.EndHorizontal();

        aimTarget = (Transform)EditorGUILayout.ObjectField("Target", aimTarget, typeof(Transform), true);
        if (aimTarget)
        {
            manipulator.TargetOrientation = aimTarget.position.normalized;
        }

        GUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.FloatField("Max Angular Velocity", rb.maxAngularVelocity);
        EditorGUILayout.Vector3Field("Center of mass (LS)", rb.centerOfMass);
        EditorGUILayout.Vector3Field("Center of mass (WS)", rb.worldCenterOfMass);
        EditorGUILayout.Vector3Field("Velocity", rb.velocity);
        EditorGUILayout.Vector3Field("Full stop acceleration", -rb.velocity / Time.fixedDeltaTime);
        EditorGUILayout.Vector3Field("Angular velocity", rb.angularVelocity);
        EditorGUILayout.Vector3Field("Full stop acceleration", -rb.angularVelocity / Time.fixedDeltaTime);
        EditorGUILayout.Separator();
        EditorGUILayout.Vector3Field("Target angular acceleration", manipulator.TargetAngularVelocity);
        EditorGUILayout.Vector3Field("Applied angular acceleration", manipulator.AppliedAngularVelocity);
        GUILayout.EndVertical();

        Repaint();
    }
}
