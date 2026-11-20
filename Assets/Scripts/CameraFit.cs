using UnityEngine;
using UnityEngine.Splines;

// Works out the view that fits the whole track between the top bar and the bottom buttons.
// CameraPanZoom starts from this view and lets the player move away from it.
[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{
    public SplineContainer track;
    public RectTransform topLimit;      // TopBarBG
    public RectTransform bottomLimit;   // BoxButton
    public float padding = 0.8f;

    public float FitSize { get; private set; }
    public Vector3 FitPosition { get; private set; }
    public Bounds TrackBounds { get; private set; }
    public event System.Action OnFit;

    int lastW, lastH;
    readonly Vector3[] corners = new Vector3[4];

    void LateUpdate()
    {
        if (Screen.width != lastW || Screen.height != lastH) Fit();
    }

    void Fit()
    {
        lastW = Screen.width;
        lastH = Screen.height;
        var cam = GetComponent<Camera>();
        Canvas.ForceUpdateCanvases();

        topLimit.GetWorldCorners(corners);
        float bandTop = Mathf.Min(corners[0].y, corners[3].y);
        bottomLimit.GetWorldCorners(corners);
        float bandBottom = Mathf.Max(corners[1].y, corners[2].y);
        float bandHeight = Mathf.Max(1f, bandTop - bandBottom);
        float bandCenter = (bandTop + bandBottom) / 2f;

        var b = new Bounds((Vector3)track.EvaluatePosition(0f), Vector3.zero);
        for (int i = 1; i <= 200; i++)
            b.Encapsulate((Vector3)track.EvaluatePosition(i / 200f));
        TrackBounds = b;

        float H = Screen.height;
        float sizeForHeight = (b.size.y + 2 * padding) * H / (2f * bandHeight);
        float sizeForWidth = (b.size.x + 2 * padding) / (2f * cam.aspect);
        FitSize = Mathf.Max(sizeForHeight, sizeForWidth);

        float unitsPerPixel = 2f * FitSize / H;
        float y = b.center.y - (bandCenter - H / 2f) * unitsPerPixel;
        FitPosition = new Vector3(b.center.x, y, cam.transform.position.z);

        cam.orthographicSize = FitSize;
        cam.transform.position = FitPosition;
        OnFit?.Invoke();
    }
}
