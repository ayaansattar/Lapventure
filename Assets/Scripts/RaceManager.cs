using UnityEngine;
using TMPro;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    public KartMover player;
    public KartMover[] rivals;
    public TMP_Text positionLabel;
    public TMP_Text leaderboardLabel;
    public int Position { get; private set; }
    public float[] leadCaps = { 0.2f, 0.6f, 1.4f, 1.8f };   // max lead per rival, slowest first
    float length;
    public Color[] rivalColors;
    public KartMover[] Order { get; private set; }
    public TeamStats stats;
    public float[] rivalPerformance = { 0.3f, 0.55f, 0.8f, 0.995f };
    public TMP_Text lapLabel;

    void Start()
    {
        length = player.track.CalculateLength();
        for (int i = 0; i < rivals.Length && i < rivalPerformance.Length; i++)
            rivals[i].speed = stats.SpeedAt(rivalPerformance[i]);
    }

    float Progress(KartMover k) => k.laps * length + k.distance;
    public int PositionOf(KartMover k)
    {
        float p = Progress(k);
        return 1 + rivals.Append(player).Count(o => o != k && Progress(o) > p);
    }

    void Update()
    {
        float p = Progress(player);
        Position = 1 + rivals.Count(r => Progress(r) > p);
        positionLabel.text = $"P{Position} / {rivals.Length + 1}";
        if (lapLabel != null) lapLabel.text = $"Lap {player.laps + 1}";


        var order = rivals.Append(player).OrderByDescending(Progress).ToArray();
        Order = order; 
        var sb = new System.Text.StringBuilder();
        int pos = 1;
        foreach (var k in order)
        {
            int idx = System.Array.IndexOf(rivals, k);
            string name = k == player
                ? "<color=#FFD700><b>YOU</b></color>"
                : idx >= 0 && idx < rivalColors.Length
                    ? $"<color=#{ColorUtility.ToHtmlStringRGB(rivalColors[idx])}>{k.name}</color>"
                    : k.name; 
            sb.AppendLine($"P{pos++}<pos=17%>{name}<pos=66%>Lap {k.laps + 1}");
        }
        leaderboardLabel.text = sb.ToString();

        for (int i = 0; i < rivals.Length; i++)
        {
            var r = rivals[i];
            float cap = leadCaps[Mathf.Min(i, leadCaps.Length - 1)]; 
            float lead = Progress(r) - p;
            float playerSpeed = player.speed * player.speedMultiplier;
            r.speedMultiplier = lead > cap * length
                ? Mathf.Min(1f, 0.95f * playerSpeed / r.speed)
                : 1f;
        }
    }
}
