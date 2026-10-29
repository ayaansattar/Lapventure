using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(LineRenderer))]
public class TrackRenderer : MonoBehaviour
{
    public SplineContainer track;
    public int samples = 200;
    public float width = 0.8f;

    void Start()
    {
        var line = GetComponent<LineRenderer>();
        line.loop = true;
        line.positionCount = samples;
        line.startWidth = line.endWidth = width;
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)samples;
            line.SetPosition(i, (Vector3)track.EvaluatePosition(t));
        }
    }
}