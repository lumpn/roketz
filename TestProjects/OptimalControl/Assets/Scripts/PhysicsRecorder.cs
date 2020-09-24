using System.IO;
using UnityEngine;

public sealed class PhysicsRecorder : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private string filename;

    private StreamWriter writer;
    private int numFrames = -1;

    void Start()
    {
        writer = File.CreateText(filename);
        writer.WriteLine("event;px;py;pz;vx;vy;vz");
    }

    void OnDestroy()
    {
        if (writer != null)
        {
            writer.Close();
            writer = null;
        }
    }

    void FixedUpdate()
    {
        if (numFrames-- < 0) return;

        var pos = rb.position;
        var vel = rb.velocity;
        writer.Write("; ");
        writer.Write(pos.x);
        writer.Write("; ");
        writer.Write(pos.y);
        writer.Write("; ");
        writer.Write(pos.z);
        writer.Write("; ");
        writer.Write(vel.x);
        writer.Write("; ");
        writer.Write(vel.y);
        writer.Write("; ");
        writer.Write(vel.z);
        writer.WriteLine();
    }

    public void Record(string evt, int numFrames)
    {
        writer.Write(evt);
        this.numFrames = numFrames;
    }
}
