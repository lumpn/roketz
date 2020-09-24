using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhysicsRecorder))]
public sealed class PhysicsRecorderEditor : Editor
{
    [SerializeField] private Vector3 force;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var recorder = (PhysicsRecorder)target;

        EditorGUILayout.BeginVertical(GUI.skin.box);
        force = EditorGUILayout.Vector3Field("Force", force);
        if (GUILayout.Button("Apply Force"))
        {
            recorder.GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
            recorder.Record(force.ToString(), 20);
        }
        EditorGUILayout.EndVertical();
    }
}
