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



    void Start() => length = player.track.CalculateLength();

    float Progress(KartMover k) => k.laps * length + k.distance;

    void Update()
    {
        float p = Progress(player);
        Position = 1 + rivals.Count(r => Progress(r) > p);
        positionLabel.text = $"P{Position} / {rivals.Length + 1}   Lap {player.laps + 1}";

        var order = rivals.Append(player).OrderByDescending(Progress);
        var sb = new System.Text.StringBuilder();
        int pos = 1;
        foreach (var k in order)
        {
            string name = k == player ? "<color=#FFD700><b>YOU</b></color>" : k.name;
            sb.AppendLine($"P{pos++}  {name}  Lap {k.laps + 1}");
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
