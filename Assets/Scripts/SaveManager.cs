using System;
using System.Collections;
using System.IO;
using UnityEngine;
using TMPro;

[DefaultExecutionOrder(-100)]
public class SaveManager : MonoBehaviour
{
    public LapPayout payout;
    public PitStopManager pitStop;
    public Department[] departments;
    public KartMover[] karts;              // PlayerKart first, then rivals
    public GameObject offlinePanel;
    public TMP_Text offlineLabel;
    public float autosaveInterval = 30f;
    public float maxOfflineHours = 2f;

    string SavePath => Path.Combine(Application.persistentDataPath, "save.json");
    float timer;
    double pendingOffline;
    long pausedAt;

    void Start()
    {
        offlinePanel.SetActive(false);
        Load();
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer >= autosaveInterval) { timer = 0; Save(); }
    }

    void OnApplicationPause(bool paused)
    {
        if (paused) { Save(); pausedAt = DateTime.UtcNow.Ticks; }
        else if (pausedAt > 0) StartCoroutine(GrantOffline(pausedAt));
    }

    void OnApplicationQuit() => Save();

    public void Save()
    {
        var d = new SaveData
        {
            money = CurrencyManager.Instance.money,
            uncollected = payout.Uncollected,
            departmentLevels = Array.ConvertAll(departments, x => x.level),
            engineerHired = payout.autoCollect,
            pitCrewHired = pitStop.autoPit,
            tires = pitStop.tires,
            kartLaps = Array.ConvertAll(karts, k => k.laps),
            kartDistances = Array.ConvertAll(karts, k => k.distance),
            lastSaveTicks = DateTime.UtcNow.Ticks
        };
        File.WriteAllText(SavePath, JsonUtility.ToJson(d));
    }

    void Load()
    {
        if (!File.Exists(SavePath)) return;
        var d = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        CurrencyManager.Instance.money = d.money;
        payout.Uncollected = d.uncollected;
        for (int i = 0; i < departments.Length && i < d.departmentLevels.Length; i++)
            departments[i].level = d.departmentLevels[i];
        payout.autoCollect = d.engineerHired;
        pitStop.autoPit = d.pitCrewHired;
        pitStop.tires = d.tires;
        for (int i = 0; i < karts.Length && i < d.kartLaps.Length; i++)
        {
            karts[i].laps = d.kartLaps[i];
            karts[i].distance = d.kartDistances[i];
        }
        StartCoroutine(GrantOffline(d.lastSaveTicks));
    }

    IEnumerator GrantOffline(long sinceTicks)
    {
        yield return null;   // wait a frame so TeamStats has set speed and income
        if (!payout.autoCollect) yield break;   // no engineer, no offline earnings
        double seconds = (DateTime.UtcNow - new DateTime(sinceTicks)).TotalSeconds;
        seconds = Math.Min(seconds, maxOfflineHours * 3600);
        if (seconds < 60) yield break;
        float length = karts[0].track.CalculateLength();
        double perSecond = payout.moneyPerLap * karts[0].speed / length;
        pendingOffline = perSecond * seconds;
        offlineLabel.text = $"While you were away, your team earned\n${NumberFormat.Format(pendingOffline)}";
        offlinePanel.SetActive(true);
    }

    public void ClaimOffline()
    {
        CurrencyManager.Instance.Add(pendingOffline);
        pendingOffline = 0;
        offlinePanel.SetActive(false);
    }

    [ContextMenu("Delete Save")]
    public void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
    }
}