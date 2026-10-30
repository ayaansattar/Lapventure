using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{
    public SplineContainer track;
    public float padding = 1f;          // world units around the track
    public float topUI = 0.15f;         // fraction of screen used by top text
    public float bottomUI = 0.28f;      // fraction of screen used by buttons

    void Start()
    {
        var cam = GetComponent<Camera>();
        var b = new Bounds((Vector3)track.EvaluatePosition(0f), Vector3.zero);
        for (int i = 1; i <= 100; i++)
            b.Encapsulate((Vector3)track.EvaluatePosition(i / 100f));

        float usable = 1f - topUI - bottomUI;
        float sizeForHeight = (b.extents.y + padding) / usable;
        float sizeForWidth = (b.extents.x + padding) / cam.aspect;
        cam.orthographicSize = Mathf.Max(sizeForHeight, sizeForWidth);

        float usableCenter = bottomUI + usable / 2f;
        float offset = (usableCenter - 0.5f) * 2f * cam.orthographicSize;
        cam.transform.position = new Vector3(b.center.x, b.center.y - offset, cam.transform.position.z);
    }
}
