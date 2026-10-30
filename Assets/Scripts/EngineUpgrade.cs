using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EngineUpgrade : MonoBehaviour
{
    public KartMover kart;
    public Button button;
    public TMP_Text buttonLabel;
    public double baseCost = 10;
    public double costGrowth = 1.15;
    public float speedPerLevel = 0.1f;   // +10% speed per level
    public int level;
    float baseSpeed;

    double Cost => baseCost * System.Math.Pow(costGrowth, level);

    void Start()
    {
        baseSpeed = kart.speed;
        CurrencyManager.Instance.OnMoneyChanged += _ => Refresh();
        Refresh();
    }

    public void Buy()
    {
        if (!CurrencyManager.Instance.TrySpend(Cost)) return;
        level++;
        kart.speed = baseSpeed * (1 + speedPerLevel * level);
        Refresh();
    }

    void Refresh()
    {
        buttonLabel.text = $"Engine Lv {level}\n${NumberFormat.Format(Cost)}";
        button.interactable = CurrencyManager.Instance.money >= Cost;
    }
}
