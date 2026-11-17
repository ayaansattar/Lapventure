using UnityEngine;
using TMPro;

public class KartInfoPopup : MonoBehaviour
{
    public static KartInfoPopup Instance { get; private set; }
    public RaceManager race;
    public RectTransform panel;
    public TMP_Text label;
    public Vector2 offset = new Vector2(0, 110);
    public float showTime = 3f;
    public float pulseAmount = 0.2f;

    KartMover target;
    Vector3 baseScale;
    float timer;
    RectTransform parentRect;
    Camera cam;

    void Awake()
    {
        Instance = this;
        parentRect = panel.parent as RectTransform;
        cam = Camera.main;
        panel.gameObject.SetActive(false);
    }

    public void Show(KartMover k)
    {
        bool sameKart = target == k && panel.gameObject.activeSelf;
        Hide();
        if (sameKart) return;              // tapping the same kart again closes it
        target = k;
        baseScale = k.transform.localScale;
        timer = showTime;
        panel.gameObject.SetActive(true);
    }

    void Hide()
    {
        if (target != null) target.transform.localScale = baseScale;
        target = null;
        panel.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (target == null) return;
        timer -= Time.deltaTime;
        if (timer <= 0) { Hide(); return; }

        float pulse = 1f + pulseAmount * Mathf.Abs(Mathf.Sin(Time.time * 6f));
        target.transform.localScale = baseScale * pulse;

        string name = target == race.player ? "YOU" : target.name;
        label.text = $"P{race.PositionOf(target)}  {name}\nLap {target.laps + 1}";

        Vector2 screen = cam.WorldToScreenPoint(target.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screen, null, out var local);
        panel.anchoredPosition = local + offset;
    }
}