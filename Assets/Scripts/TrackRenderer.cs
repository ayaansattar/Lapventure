using UnityEngine;
using UnityEngine.Splines;

// Builds the visible track from the spline: kerbs, asphalt, and a start/finish line.
public class TrackRenderer : MonoBehaviour
{
    public SplineContainer track;
    public int samples = 300;

    [Header("Size")]
    public float width = 1.4f;              // asphalt width
    public float kerbWidth = 0.18f;         // kerb visible on each side
    public float kerbStripeLength = 0.35f;  // length of one red or white stripe

    [Header("Look")]
    public Material baseMaterial;           // e.g. Sprite-Unlit-Default
    public Texture2D kerbTexture;
    public Texture2D asphaltTexture;
    public Color asphaltColor = new Color(0.25f, 0.25f, 0.27f);
    public Sprite startLineSprite;

    void Start()
    {
        var pts = new Vector3[samples];
        for (int i = 0; i < samples; i++)
            pts[i] = (Vector3)track.EvaluatePosition(i / (float)samples);

        MakeLine("Kerbs", pts, width + kerbWidth * 2f, kerbTexture, Color.white,
                 1f / (kerbStripeLength * 2f), -2);
        MakeLine("Asphalt", pts, width, asphaltTexture, asphaltColor, 1f / width, -1);
        MakeStartLine();
    }

    void MakeLine(string name, Vector3[] pts, float w, Texture2D tex, Color color, float tiling, int order)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var line = go.AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.loop = true;
        line.positionCount = pts.Length;
        line.SetPositions(pts);
        line.startWidth = line.endWidth = w;
        line.numCornerVertices = 4;
        line.startColor = line.endColor = color;

        var mat = new Material(baseMaterial);
        if (tex != null)
        {
            mat.mainTexture = tex;
            line.textureMode = LineTextureMode.Tile;
            line.textureScale = new Vector2(tiling, 1f);
        }
        line.material = mat;
        line.sortingOrder = order;
    }

    void MakeStartLine()
    {
        if (startLineSprite == null) return;
        var go = new GameObject("StartLine");
        go.transform.SetParent(transform, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = startLineSprite;
        sr.sortingOrder = 0;

        Vector3 pos = (Vector3)track.EvaluatePosition(0f);
        Vector3 dir = ((Vector3)track.EvaluateTangent(0f)).normalized;
        go.transform.position = pos;
        go.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        float s = width / startLineSprite.bounds.size.x;
        go.transform.localScale = new Vector3(s, s, 1f);
    }
}
