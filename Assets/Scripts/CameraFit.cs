using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{
    public SplineContainer track;
    public RectTransform topLimit;      // lowest element of the top HUD (TireText)
    public RectTransform bottomLimit;   // highest element of the bottom UI (BoxButton)
    public float padding = 0.8f;        // world units of grass around the track

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

        // Screen-pixel band between the top HUD and the bottom buttons
        topLimit.GetWorldCorners(corners);
        float bandTop = Mathf.Min(corners[0].y, corners[3].y);
        bottomLimit.GetWorldCorners(corners);
        float bandBottom = Mathf.Max(corners[1].y, corners[2].y);
        float bandHeight = Mathf.Max(1f, bandTop - bandBottom);
        float bandCenter = (bandTop + bandBottom) / 2f;

        // Track bounds in world space
        var b = new Bounds((Vector3)track.EvaluatePosition(0f), Vector3.zero);
        for (int i = 1; i <= 200; i++)
            b.Encapsulate((Vector3)track.EvaluatePosition(i / 200f));

        float H = Screen.height;
        float sizeForHeight = (b.size.y + 2 * padding) * H / (2f * bandHeight);
        float sizeForWidth = (b.size.x + 2 * padding) / (2f * cam.aspect);
        cam.orthographicSize = Mathf.Max(sizeForHeight, sizeForWidth);

        float unitsPerPixel = 2f * cam.orthographicSize / H;
        float y = b.center.y - (bandCenter - H / 2f) * unitsPerPixel;
        cam.transform.position = new Vector3(b.center.x, y, cam.transform.position.z);
    }
}