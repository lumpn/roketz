using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(CaveMesher))]
public class CaveMesherEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();

        var mesher = (CaveMesher)target;

        if (GUILayout.Button ("Regenerate")) {
            mesher.Generate ();
        }
    }
}
