using UnityEngine;

// Steers a kart into a pit lane beside the start/finish straight, stops it at the pit box,
// then merges it back onto the track. Distances are measured from the start/finish line
// (negative = before the line, positive = after it).
[RequireComponent(typeof(KartMover))]
public class PitLane : MonoBehaviour {
    [Header("Layout (track units from the start/finish line)")]
    public float side = 1f;            // 1 = left of travel direction, -1 = right
    public float laneOffset = 1.8f;    // distance from the racing line to the pit lane center
    public float entryStart = -3.5f;   // start turning into the pit lane
    public float entryEnd = -2f;       // fully in the pit lane
    public float boxAt = 1f;           // where the kart stops
    public float exitStart = 2.5f;     // start merging back
    public float exitEnd = 4f;         // back on the racing line

    [Header("Driving")]
    public float laneSpeed = 0.5f;     // pit lane speed limit, as a share of normal speed

    public bool Busy => requested || inLane;

    KartMover kart;
    float length;
    bool requested, inLane, stopped;
    System.Action onArrive;

    void Start() {
        kart = GetComponent<KartMover>();
        length = kart.track.CalculateLength();
    }

    float S => kart.distance > length / 2f ? kart.distance - length : kart.distance;

    // 0 on the racing line, 1 fully in the pit lane, at a given distance from the line
    public float Blend(float s) {
        float b;
        if (s < entryStart || s > exitEnd) b = 0f;
        else if (s < entryEnd) b = Mathf.InverseLerp(entryStart, entryEnd, s);
        else if (s < exitStart) b = 1f;
        else b = 1f - Mathf.InverseLerp(exitStart, exitEnd, s);
        return Mathf.SmoothStep(0f, 1f, b);
    }

    public float OffsetAt(float s) => side * laneOffset * Blend(s);

    public void Request(System.Action arrive) {
        requested = true;
        onArrive = arrive;
    }

    void Update() {
        float s = S;

        // Only turn in at the start of the entry, so a late call waits for the next lap
        if (requested && !inLane && s >= entryStart && s < entryStart + 0.6f) {
            inLane = true;
            requested = false;
            stopped = false;
        }

        if (inLane) {
            if (!stopped && s >= boxAt && s < exitStart) {
                stopped = true;
                kart.distance = boxAt;
                kart.paused = true;
                onArrive?.Invoke();
            }
            if (stopped && s >= exitEnd) inLane = false;
        }

        kart.lateralOffset = inLane ? OffsetAt(s) : 0f;
        kart.laneFactor = inLane ? laneSpeed : 1f;
    }
}
