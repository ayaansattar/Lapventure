using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PitStopManager : MonoBehaviour
{
    public KartMover kart;
    public Button boxButton;
    public GameObject timingPanel;
    public Slider timingBar;
    public TMP_Text tireLabel;

    public float wearPerLap = 12f;
    public float lowWear = 30f;          // below this, kart slows down
    public float wornSpeed = 0.6f;       // 60% speed when worn
    public float markerSpeed = 1.5f;     // how fast the marker slides
    public float greenMin = 0.4f, greenMax = 0.6f;
    public float boostMultiplier = 1.3f, boostDuration = 10f;

    float tires = 100f;
    float boostTimer;
    bool pitting;

    void Start()
    {
        kart.OnLapCompleted += () => tires = Mathf.Max(0, tires - wearPerLap);
        timingPanel.SetActive(false);
    }

    void Update()
    {
        if (pitting)
        {
            timingBar.value = Mathf.PingPong(Time.time * markerSpeed, 1f);
            return;
        }
        boostTimer -= Time.deltaTime;
        float mult = tires < lowWear ? wornSpeed : 1f;
        if (boostTimer > 0) mult *= boostMultiplier;
        kart.speedMultiplier = mult;

        boxButton.gameObject.SetActive(tires < 50f);
        tireLabel.text = $"Tires {tires:0}%" + (boostTimer > 0 ? "  BOOST!" : "");
    }

    public void StartPit()
    {
        pitting = true;
        kart.paused = true;
        boxButton.gameObject.SetActive(false);
        timingPanel.SetActive(true);
    }

    public void StopMarker()
    {
        float v = timingBar.value;
        bool perfect = v >= greenMin && v <= greenMax;
        StartCoroutine(FinishPit(perfect ? 0.5f : 2f, perfect));
    }

    IEnumerator FinishPit(float delay, bool perfect)
    {
        timingPanel.SetActive(false);
        yield return new WaitForSeconds(delay);
        tires = 100f;
        if (perfect) boostTimer = boostDuration;
        kart.paused = false;
        pitting = false;
    }
}
