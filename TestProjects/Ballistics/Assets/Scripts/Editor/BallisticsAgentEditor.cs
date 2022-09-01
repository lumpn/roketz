using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BallisticsAgent))]
public sealed class BallisticsAgentEditor : Editor
{
    private Vector3 rotation, velocity;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var agent = (BallisticsAgent)target;

        EditorGUILayout.BeginVertical(GUI.skin.box);

        rotation = EditorGUILayout.Vector3Field("Rotation", rotation);
        velocity = EditorGUILayout.Vector3Field("Velocity", velocity);

        if (GUILayout.Button("Draw Trajectory"))
        {
            agent.DrawTrajectory(Quaternion.Euler(rotation), velocity);
        }

        EditorGUILayout.EndVertical();
    }
}
