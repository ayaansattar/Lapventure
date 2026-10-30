using UnityEngine;

public class TeamStats : MonoBehaviour
{
    public KartMover kart;
    public LapPayout payout;
    public Department[] departments;
    public float baseSpeed = 2f;
    public double baseIncome = 5;

    void Start()
    {
        foreach (var d in departments) d.OnUpgraded += Recalculate;
        Recalculate();
    }

    void Recalculate()
    {
        float speed = baseSpeed;
        double income = baseIncome;
        foreach (var d in departments)
        {
            speed *= d.SpeedMultiplier;
            income += d.IncomeBonus;
        }
        kart.speed = speed;
        payout.moneyPerLap = income;
    }
}
