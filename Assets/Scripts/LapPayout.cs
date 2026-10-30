using UnityEngine;

[RequireComponent(typeof(KartMover))]
public class LapPayout : MonoBehaviour
{
    public double moneyPerLap = 5;
    void Start() => GetComponent<KartMover>().OnLapCompleted += Pay;
    void Pay() => CurrencyManager.Instance.Add(moneyPerLap);
}
