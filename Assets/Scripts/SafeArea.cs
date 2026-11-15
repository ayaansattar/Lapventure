using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    void Start()
    {
        var rt = GetComponent<RectTransform>();
        Rect s = Screen.safeArea;
        rt.anchorMin = new Vector2(s.xMin / Screen.width, s.yMin / Screen.height);
        rt.anchorMax = new Vector2(s.xMax / Screen.width, s.yMax / Screen.height);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}