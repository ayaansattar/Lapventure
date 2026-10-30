using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    public double money;
    public event System.Action<double> OnMoneyChanged;

    void Awake() => Instance = this;

    public void Add(double amount) { money += amount; OnMoneyChanged?.Invoke(money); }

    public bool TrySpend(double amount)
    {
        if (money < amount) return false;
        money -= amount; OnMoneyChanged?.Invoke(money); return true;
    }
}
