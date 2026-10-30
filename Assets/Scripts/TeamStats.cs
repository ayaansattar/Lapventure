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
        float bonus = 0f;
        double income = baseIncome;
        foreach (var d in departments)
        {
            bonus += d.SpeedMultiplier - 1f;
            income += d.IncomeBonus;
        }
        kart.speed = baseSpeed * (1f + bonus);
        payout.moneyPerLap = income;
    }
}
