using UnityEngine;

// Turns the four departments into the kart's speed and money per lap.
public class TeamStats : MonoBehaviour {
    public KartMover kart;
    public LapPayout payout;
    public Department[] departments;
    public float startSpeed = 4f;    // speed with every department at level 0
    public float maxSpeed = 12f;     // speed with every department at max level
    public double baseIncome = 5;

    // 0..1: share of all department levels bought
    public float Performance {
        get {
            int total = 0, max = 0;
            foreach (var d in departments) { total += d.level; max += d.config.maxLevel; }
            return max == 0 ? 0f : total / (float)max;
        }
    }

    public float SpeedAt(float performance) => Mathf.Lerp(startSpeed, maxSpeed, performance);

    void Start() {
        foreach (var d in departments) d.OnUpgraded += Recalculate;
        Recalculate();
    }

    public void Recalculate() {
        kart.speed = SpeedAt(Performance);
        double income = baseIncome;
        foreach (var d in departments) income += d.Income;
        payout.moneyPerLap = income;
    }
}
