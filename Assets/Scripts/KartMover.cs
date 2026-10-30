using UnityEngine;
using UnityEngine.Splines;

public class KartMover : MonoBehaviour
{
    public SplineContainer track;
    public float speed = 2f;          // units per second
    public int laps;
    public float distance;
    float length;
    public event System.Action OnLapCompleted;

    void Start() => length = track.CalculateLength();

    void Update()
    {
        distance += speed * Time.deltaTime;
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
