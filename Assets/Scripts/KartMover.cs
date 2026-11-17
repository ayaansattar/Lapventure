using UnityEngine;
using UnityEngine.Splines;

public class KartMover : MonoBehaviour
{
    public SplineContainer track;
    public float speed = 2f;
    public int laps;
    public float distance;
    public float speedMultiplier = 1f;
    public bool paused;
    [HideInInspector] public float lateralOffset;    // sideways shift (pit lane)
    [HideInInspector] public float laneFactor = 1f;  // pit lane speed limit
    public event System.Action OnLapCompleted;

    float length;
    float lastOffset;

    void Start() => length = track.CalculateLength();

    void Update()
    {
        if (paused) return;
        float step = speed * speedMultiplier * laneFactor * Time.deltaTime;
        distance += step;
        if (distance >= length)
        {
            distance -= length;
            laps++;
            OnLapCompleted?.Invoke();
        }
        float t = distance / length;
        Vector3 tan = ((Vector3)track.EvaluateTangent(t)).normalized;
        Vector3 nrm = new Vector3(-tan.y, tan.x, 0f);
        transform.position = (Vector3)track.EvaluatePosition(t) + nrm * lateralOffset;

        // Angle the kart while it moves sideways into or out of the pit lane
        float slope = step > 1e-5f ? (lateralOffset - lastOffset) / step : 0f;
        lastOffset = lateralOffset;
        transform.up = (tan + nrm * slope).normalized;
    }
}
