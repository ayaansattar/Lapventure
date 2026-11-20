using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

// Pinch or scroll to zoom, drag to move around the circuit.
// Starts from the view CameraFit calculates, and keeps the camera near the track.
[RequireComponent(typeof(Camera), typeof(CameraFit))]
public class CameraPanZoom : MonoBehaviour {
    public float startZoom = 1.15f;     // 1 = CameraFit's view, higher = further out
    public float minZoom = 0.45f;       // closest, as a share of the fit size
    public float maxZoom = 1.6f;        // furthest
    public float panMargin = 4f;        // how far past the track you can drag (world units)
    public float scrollSpeed = 0.1f;    // mouse wheel, for testing in the editor

    Camera cam;
    CameraFit fit;
    bool dragging;
    Vector2 lastMouse;
    readonly List<RaycastResult> hits = new List<RaycastResult>();

    void Awake() {
        cam = GetComponent<Camera>();
        fit = GetComponent<CameraFit>();
        fit.OnFit += ResetView;
    }

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    // Hook a "recenter" button to this if you like
    public void ResetView() {
        cam.orthographicSize = fit.FitSize * startZoom;
        transform.position = fit.FitPosition;
        Clamp();
    }

    void LateUpdate() {
        if (fit.FitSize <= 0f) return;
        var touches = Touch.activeTouches;

        if (touches.Count >= 2) {
            var a = touches[0]; var b = touches[1];
            Vector2 pa = a.screenPosition, pb = b.screenPosition;
            float now = Vector2.Distance(pa, pb);
            float before = Vector2.Distance(pa - a.delta, pb - b.delta);
            if (before > 1f) ZoomAt((pa + pb) / 2f, before / now);
            dragging = false;
        } else if (touches.Count == 1) {
            var t = touches[0];
            if (t.phase == UnityEngine.InputSystem.TouchPhase.Began) dragging = !OverUI(t.screenPosition);
            else if (dragging && t.phase == UnityEngine.InputSystem.TouchPhase.Moved) Pan(t.delta);
        } else if (Mouse.current != null) {
            var m = Mouse.current;
            Vector2 pos = m.position.ReadValue();
            if (m.leftButton.wasPressedThisFrame) dragging = !OverUI(pos);
            if (m.leftButton.wasReleasedThisFrame) dragging = false;
            if (dragging && m.leftButton.isPressed) Pan(pos - lastMouse);
            lastMouse = pos;

            float scroll = m.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f && !OverUI(pos))
                ZoomAt(pos, scroll > 0 ? 1f - scrollSpeed : 1f + scrollSpeed);
        }
    }

    void Pan(Vector2 screenDelta) {
        float unitsPerPixel = 2f * cam.orthographicSize / Screen.height;
        transform.position -= (Vector3)(screenDelta * unitsPerPixel);
        Clamp();
    }

    void ZoomAt(Vector2 screenPoint, float factor) {
        Vector3 before = cam.ScreenToWorldPoint(screenPoint);
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize * factor,
                                           fit.FitSize * minZoom, fit.FitSize * maxZoom);
        Vector3 after = cam.ScreenToWorldPoint(screenPoint);
        transform.position += before - after;
        Clamp();
    }

    void Clamp() {
        Bounds b = fit.TrackBounds;
        Vector3 off = fit.FitPosition - b.center;     // keeps the HUD offset CameraFit chose
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, b.min.x - panMargin + off.x, b.max.x + panMargin + off.x);
        p.y = Mathf.Clamp(p.y, b.min.y - panMargin + off.y, b.max.y + panMargin + off.y);
        transform.position = p;
    }

    // True when the pointer is over a UI element (buttons, panels), so dragging there doesn't pan
    bool OverUI(Vector2 pos) {
        if (EventSystem.current == null) return false;
        var data = new PointerEventData(EventSystem.current) { position = pos };
        hits.Clear();
        EventSystem.current.RaycastAll(data, hits);
        foreach (var h in hits)
            if (h.module is UnityEngine.UI.GraphicRaycaster) return true;
        return false;
    }
}
