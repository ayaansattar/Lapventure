using UnityEngine;

// Draws the pit lane asphalt and the pit box next to the start/finish straight.
public class PitLaneRenderer : MonoBehaviour {
    public PitLane pitLane;              // the PitLane on PlayerKart
    public Material baseMaterial;        // same material as the Track Renderer
    public Texture2D asphaltTexture;
    public Color laneColor = new Color(0.32f, 0.32f, 0.35f);
    public Color boxColor = new Color(1f, 0.84f, 0f, 0.85f);
    public float laneWidth = 1.2f;
    public int samples = 80;

    void Start() {
        var track = pitLane.GetComponent<KartMover>().track;
        float length = track.CalculateLength();

        var pts = new Vector3[samples];
        for (int i = 0; i < samples; i++) {
            float s = Mathf.Lerp(pitLane.entryStart, pitLane.exitEnd, i / (samples - 1f));
            pts[i] = PointAt(track, length, s, pitLane.OffsetAt(s));
        }
        MakeLine("PitLane", pts, laneWidth, laneColor, -1, asphaltTexture);

        float o = pitLane.side * pitLane.laneOffset;
        var box = new[] {
            PointAt(track, length, pitLane.boxAt - 0.45f, o),
            PointAt(track, length, pitLane.boxAt + 0.45f, o),
        };
        MakeLine("PitBox", box, laneWidth * 0.9f, boxColor, 0, null);
    }

    static Vector3 PointAt(UnityEngine.Splines.SplineContainer track, float length, float s, float offset) {
        float d = s < 0 ? length + s : s;
        float t = d / length;
        Vector3 pos = (Vector3)track.EvaluatePosition(t);
        Vector3 tan = ((Vector3)track.EvaluateTangent(t)).normalized;
        Vector3 nrm = new Vector3(-tan.y, tan.x, 0f);
        return pos + nrm * offset;
    }

    void MakeLine(string name, Vector3[] pts, float w, Color color, int order, Texture2D tex) {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var line = go.AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.positionCount = pts.Length;
        line.SetPositions(pts);
        line.startWidth = line.endWidth = w;
        line.numCapVertices = 2;
        line.numCornerVertices = 4;
        line.startColor = line.endColor = color;
        var mat = new Material(baseMaterial);
        if (tex != null) {
            mat.mainTexture = tex;
            line.textureMode = LineTextureMode.Tile;
            line.textureScale = new Vector2(1f / w, 1f);
        }
        line.material = mat;
        line.sortingOrder = order;
    }
}
