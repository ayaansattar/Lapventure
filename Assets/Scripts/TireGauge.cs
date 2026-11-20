using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Round tire-wear gauge: the ring empties as the tires wear and turns
// green → yellow → red. Pulses when it's time to pit.
public class TireGauge : MonoBehaviour {
    public PitStopManager pitStop;
    public Image ring;                  // Image Type: Filled, Radial 360
    public TMP_Text label;              // "72%"
    public GameObject boostTag;         // optional "BOOST" label
    public Color good = new Color(0.30f, 0.80f, 0.35f);
    public Color worn = new Color(1.00f, 0.80f, 0.15f);
    public Color bad  = new Color(0.90f, 0.25f, 0.20f);

    void Update() {
        float t = pitStop.tires / 100f;
        ring.fillAmount = t;
        ring.color = pitStop.tires > 50f ? good : pitStop.tires > pitStop.boxAt ? worn : bad;
        label.text = $"{pitStop.tires:0}%";
        if (boostTag != null) boostTag.SetActive(pitStop.Boosting);

        float pulse = pitStop.tires <= pitStop.boxAt ? 1f + 0.08f * Mathf.Abs(Mathf.Sin(Time.time * 5f)) : 1f;
        transform.localScale = new Vector3(pulse, pulse, 1f);
    }
}
