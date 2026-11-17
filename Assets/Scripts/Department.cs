using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Department : MonoBehaviour {
    public DepartmentConfig config;
    public Button button;
    public TMP_Text label;
    public Image progressFill;               // optional: Image Type = Filled, shows progress to next star
    public int level;
    [HideInInspector] public double upgradeMultiplier = 1;   // set by TeamUpgradesManager
    public event Action OnUpgraded;

    public bool IsMaxed => level >= config.maxLevel;
    public double Cost => config.baseCost * Math.Pow(config.costGrowth, level);

    public int Stars {
        get { int s = 0; foreach (int m in config.stars) if (level >= m) s++; return s; }
    }
    public double StarMultiplier => Math.Pow(config.starMultiplier, Stars);
    public double Income => config.incomePerLevel * level * StarMultiplier * upgradeMultiplier;

    // 0..1 progress from the previous star to the next one
    public float StarProgress {
        get {
            int prev = 0;
            foreach (int m in config.stars) {
                if (level < m) return (level - prev) / (float)(m - prev);
                prev = m;
            }
            return 1f;
        }
    }

    void Start() {
        CurrencyManager.Instance.OnMoneyChanged += OnMoney;
        BuyMode.Changed += Refresh;
        Refresh();
    }

    void OnDestroy() {
        BuyMode.Changed -= Refresh;
        if (CurrencyManager.Instance != null) CurrencyManager.Instance.OnMoneyChanged -= OnMoney;
    }

    void OnMoney(double _) => Refresh();

    // How many levels the current buy mode would purchase
    int LevelsToBuy() {
        int left = config.maxLevel - level;
        if (BuyMode.Amount > 0) return Mathf.Min(BuyMode.Amount, left);
        int n = 0; double sum = 0, c = Cost, money = CurrencyManager.Instance.money;
        while (n < left && sum + c <= money) { sum += c; c *= config.costGrowth; n++; }
        return Mathf.Max(1, Mathf.Min(n, left));
    }

    double CostFor(int n) =>
        Cost * (Math.Pow(config.costGrowth, n) - 1) / (config.costGrowth - 1);

    public void Buy() {
        if (IsMaxed) return;
        int n = LevelsToBuy();
        if (!CurrencyManager.Instance.TrySpend(CostFor(n))) return;
        level += n;
        OnUpgraded?.Invoke();
        Refresh();
    }

    public void Refresh() {
        if (config == null || label == null) return;
        string mult = Stars > 0 ? $" <color=#C79A00>x{StarMultiplier:0}</color>" : "";
        if (IsMaxed) {
            label.text = $"{config.displayName}{mult}\nMAX";
            button.interactable = false;
        } else {
            int n = LevelsToBuy();
            double c = CostFor(n);
            string amount = n > 1 ? $" (+{n})" : "";
            label.text = $"{config.displayName} Lv {level}{mult}\n${NumberFormat.Format(c)}{amount}";
            button.interactable = CurrencyManager.Instance.money >= c;
        }
        if (progressFill != null) progressFill.fillAmount = StarProgress;
    }
}
