using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Department : MonoBehaviour
{
    public DepartmentConfig config;
    public Button button;
    public TMP_Text label;
    public int level;
    public event System.Action OnUpgraded;

    public double Cost => config.baseCost * System.Math.Pow(config.costGrowth, level);

    float MilestoneBonus
    {
        get
        {
            float bonus = 1f;
            foreach (int m in config.milestones)
                if (level >= m) bonus *= config.milestoneMultiplier;
            return bonus;
        }
    }

    public float SpeedMultiplier => 1f + config.speedPerLevel * level * MilestoneBonus;
    public double IncomeBonus => config.incomePerLevel * level * MilestoneBonus;

    void Start()
    {
        CurrencyManager.Instance.OnMoneyChanged += _ => Refresh();
        Refresh();
    }

    public void Buy()
    {
        if (!CurrencyManager.Instance.TrySpend(Cost)) return;
        level++;
        OnUpgraded?.Invoke();
        Refresh();
    }

    void Refresh()
    {
        label.text = $"{config.displayName} Lv {level}\n${NumberFormat.Format(Cost)}";
        button.interactable = CurrencyManager.Instance.money >= Cost;
    }
}
