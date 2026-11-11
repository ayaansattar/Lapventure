using UnityEngine;

[RequireComponent(typeof(KartMover))]
public class LapPayout : MonoBehaviour
{
    public double moneyPerLap = 5;
    public bool autoCollect;
    public double Uncollected { get; private set; }
    public event System.Action<double> OnUncollectedChanged;

    void Start() => GetComponent<KartMover>().OnLapCompleted += Pay;

    void Pay()
    {
        if (autoCollect) CurrencyManager.Instance.Add(moneyPerLap);
        else { Uncollected += moneyPerLap; OnUncollectedChanged?.Invoke(Uncollected); }
    }

    public void Collect()
    {
        if (Uncollected <= 0) return;
        CurrencyManager.Instance.Add(Uncollected);
        Uncollected = 0;
        OnUncollectedChanged?.Invoke(Uncollected);
    }
}