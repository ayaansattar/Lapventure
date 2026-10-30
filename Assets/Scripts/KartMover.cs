using UnityEngine;
using UnityEngine.Splines;

public class KartMover : MonoBehaviour
{
    public SplineContainer track;
    public float speed = 2f;          // units per second
    public int laps;
    public float distance;
    public float speedMultiplier = 1f;
    public bool paused;
    float length;
    public event System.Action OnLapCompleted;

    void Start() => length = track.CalculateLength();

    void Update()
    {
        if (paused) return;
        distance += speed * speedMultiplier * Time.deltaTime;

        if (distance >= length)
        {
            distance -= length;
            laps++;
            OnLapCompleted?.Invoke();
        }
        float t = distance / length;
        transform.position = (Vector3)track.EvaluatePosition(t);
        transform.up = ((Vector3)track.EvaluateTangent(t)).normalized;
    }
}
