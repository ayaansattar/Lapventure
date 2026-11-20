using UnityEngine;

// Stretches the dark top bar from the very top of the screen (behind the notch)
// down to the bottom edge of the top bar content inside SafeArea.
[RequireComponent(typeof(RectTransform))]
public class TopBarBackground : MonoBehaviour {
    public RectTransform content;    // the TopBar object inside SafeArea
    public float extra = 0f;         // extra height below the content, in canvas units

    RectTransform rt;
    Canvas canvas;
    readonly Vector3[] corners = new Vector3[4];
    int lastW, lastH;

    void Awake() {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void LateUpdate() {
        if (Screen.width == lastW && Screen.height == lastH) return;
        lastW = Screen.width; lastH = Screen.height;
        Canvas.ForceUpdateCanvases();
        content.GetWorldCorners(corners);
        float bottomPx = Mathf.Min(corners[0].y, corners[3].y);
        float height = (Screen.height - bottomPx) / canvas.scaleFactor + extra;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
    }
}
