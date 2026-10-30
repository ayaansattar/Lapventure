using UnityEngine;
using TMPro;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    public KartMover player;
    public KartMover[] rivals;
    public TMP_Text positionLabel;
    public int Position { get; private set; }
    float length;

    void Start() => length = player.track.CalculateLength();

    float Progress(KartMover k) => k.laps * length + k.distance;

    void Update()
    {
        float p = Progress(player);
        Position = 1 + rivals.Count(r => Progress(r) > p);
        positionLabel.text = $"P{Position} / {rivals.Length + 1}";
    }
}
