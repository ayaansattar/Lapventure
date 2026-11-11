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
    }
}
