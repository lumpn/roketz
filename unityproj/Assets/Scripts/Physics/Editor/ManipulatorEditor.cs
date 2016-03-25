using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Manipulator))]
public class ManipulatorEditor : Editor
{
    private Vector3 velocity;
    private Vector3 acceleration;

    private Vector3 angularVelocity;
    private Vector3 angularAcceleration;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var manipulator = (Manipulator)target;

        GUILayout.BeginHorizontal();
        velocity = EditorGUILayout.Vector3Field("Velocity", velocity);
        if (GUILayout.Button("Set", GUILayout.Width(80)))
        {
            manipulator.rb.velocity = velocity;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        acceleration = EditorGUILayout.Vector3Field("Acceleration", acceleration);
        if (GUILayout.Button("Apply", GUILayout.Width(80)))
        {
            manipulator.rb.AddForce(acceleration, ForceMode.Acceleration);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        angularVelocity = EditorGUILayout.Vector3Field("Angular velocity", angularVelocity);
        if (GUILayout.Button("Set", GUILayout.Width(80)))
        {
            manipulator.rb.angularVelocity = angularVelocity;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        angularAcceleration = EditorGUILayout.Vector3Field("Angular acceleration", angularAcceleration);
        if (GUILayout.Button("Apply", GUILayout.Width(80)))
        {
            manipulator.rb.AddTorque(angularAcceleration, ForceMode.Acceleration);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.FloatField("Max Angular Velocity", manipulator.rb.maxAngularVelocity);
        EditorGUILayout.Vector3Field("Center of mass (LS)", manipulator.rb.centerOfMass);
        EditorGUILayout.Vector3Field("Center of mass (WS)", manipulator.rb.worldCenterOfMass);
        EditorGUILayout.Vector3Field("Velocity", manipulator.rb.velocity);
        EditorGUILayout.Vector3Field("Full stop acceleration", -manipulator.rb.velocity / Time.fixedDeltaTime);
        EditorGUILayout.Vector3Field("Angular velocity", manipulator.rb.angularVelocity);
        EditorGUILayout.Vector3Field("Full stop acceleration", -manipulator.rb.angularVelocity / Time.fixedDeltaTime);
        EditorGUILayout.Vector3Field("Applied angular acceleration", manipulator.appliedAngularVelocity);
        GUILayout.EndVertical();
    }
}
