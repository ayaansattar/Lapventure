using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChampionshipManager : MonoBehaviour
{
    public RaceManager race;
    public KartMover player;
    public Slider progressBar;
    public TMP_Text goalLabel;
    public GameObject winPanel;

    float length;
    float leadStart = -1f;
    bool won;

    void Start()
    {
        length = player.track.CalculateLength();
        winPanel.SetActive(false);
    }

    float PlayerProgress => player.laps * length + player.distance;

    void Update()
    {
        if (won) return;
        int karts = race.rivals.Length + 1;

        if (race.Position == 1)
        {
            if (leadStart < 0) leadStart = PlayerProgress;
            float held = Mathf.Clamp01((PlayerProgress - leadStart) / length);
            progressBar.value = 0.8f + 0.2f * held;
            goalLabel.text = "Hold the lead for one lap!";
            if (held >= 1f) Win();
        }
        else
        {
            leadStart = -1f;
            progressBar.value = 0.8f * (karts - race.Position) / (karts - 1);
            goalLabel.text = $"Overtake to P1 ({race.Position - 1} to pass)";
        }
    }

    void Win()
    {
        won = true;
        progressBar.value = 1f;
        goalLabel.text = "Champion!";
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseWinPanel()
    {
        winPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
