using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PitStopManager : MonoBehaviour
{
    public KartMover kart;
    public PitLane pitLane;              // the PitLane on PlayerKart
    public Button boxButton;
    public TMP_Text boxLabel;            // optional: BOX BOX button's text
    public GameObject timingPanel;
    public Slider timingBar;
    public TMP_Text tireLabel;

    public float wearPerLap = 10f;
    public float lowWear = 30f;
    public float wornSpeed = 0.6f;
    public float markerSpeed = 0.7f;
    public float greenMin = 0.4f, greenMax = 0.6f;
    public float boostMultiplier = 1.3f, boostDuration = 10f;
    public float boxAt = 30f;
    public float autoPitAt = 20f;

    public bool autoPit;
    public int crewSize = 1;
    public float baseStopTime = 2f;
    public float perfectStopTime = 0.5f;
    float StopTime => baseStopTime / (1f + 0.5f * (crewSize - 1));
    public bool Boosting => boostTimer > 0;

    [HideInInspector] public float tires = 100f;
    float boostTimer;
    bool pitting;        // stopped in the pit box
    bool pitCalled;      // heading for the pit lane

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
        if (autoPit && !pitCalled && tires <= autoPitAt) CallPit(manual: false);

        boostTimer -= Time.deltaTime;
        float mult = tires < lowWear ? wornSpeed : 1f;
        if (boostTimer > 0) mult *= boostMultiplier;
        kart.speedMultiplier = mult;

        boxButton.interactable = !pitCalled && tires <= boxAt;
        if (boxLabel != null) boxLabel.text = pitCalled ? "PIT THIS LAP" : "BOX BOX";
        if (tireLabel != null) tireLabel.text = $"Tires {tires:0}%" + (boostTimer > 0 ? "  BOOST!" : "");
    }

    // BOX BOX button
    public void StartPit()
    {
        if (!pitCalled) CallPit(manual: true);
    }

    void CallPit(bool manual)
    {
        pitCalled = true;
        pitLane.Request(() => ArriveAtBox(manual));
    }

    void ArriveAtBox(bool manual)
    {
        pitting = true;
        if (manual) timingPanel.SetActive(true);
        else StartCoroutine(FinishPit(StopTime, false));
    }

    // STOP! button on the timing bar
    public void StopMarker()
    {
        float v = timingBar.value;
        bool perfect = v >= greenMin && v <= greenMax;
        StartCoroutine(FinishPit(perfect ? perfectStopTime : StopTime, perfect));
    }

    IEnumerator FinishPit(float delay, bool perfect)
    {
        timingPanel.SetActive(false);
        yield return new WaitForSeconds(delay);
        tires = 100f;
        if (perfect) boostTimer = boostDuration;
        kart.paused = false;
        pitting = false;
        pitCalled = false;
    }
}
