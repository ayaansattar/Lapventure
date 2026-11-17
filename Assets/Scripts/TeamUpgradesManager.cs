using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TeamUpgrade {
    public string title;
    public string description;
    public Sprite icon;
    public double cost;
    public int department = -1;      // 0 Engine, 1 Tires, 2 Aero, 3 Chassis, -1 = all
    public double multiplier = 2;
    [HideInInspector] public bool purchased;
}

// One-time upgrades that multiply department income (the "Upgrades" popup).
public class TeamUpgradesManager : MonoBehaviour {
    public TeamStats stats;
    public GameObject panel;
    public Transform listParent;         // the ScrollView's Content
    public UpgradeRow rowPrefab;
    public GameObject badge;             // optional "!" shown when something is affordable
    public int visibleCount = 5;

    public List<TeamUpgrade> upgrades = new List<TeamUpgrade> {
        new TeamUpgrade { title = "Racing Fuel",          description = "Engine prize x2",  cost = 150,   department = 0,  multiplier = 2 },
        new TeamUpgrade { title = "Soft Compound",        description = "Tires prize x2",   cost = 400,   department = 1,  multiplier = 2 },
        new TeamUpgrade { title = "Wing Endplates",       description = "Aero prize x2",    cost = 1e3,   department = 2,  multiplier = 2 },
        new TeamUpgrade { title = "Carbon Floor",         description = "Chassis prize x2", cost = 2.5e3, department = 3,  multiplier = 2 },
        new TeamUpgrade { title = "Energy Drink Sponsor", description = "All prize x2",     cost = 8e3,   department = -1, multiplier = 2 },
        new TeamUpgrade { title = "Turbo Intake",         description = "Engine prize x3",  cost = 2.5e4, department = 0,  multiplier = 3 },
        new TeamUpgrade { title = "Tire Blankets",        description = "Tires prize x3",   cost = 6e4,   department = 1,  multiplier = 3 },
        new TeamUpgrade { title = "Rear Diffuser",        description = "Aero prize x3",    cost = 1.5e5, department = 2,  multiplier = 3 },
        new TeamUpgrade { title = "Lightweight Frame",    description = "Chassis prize x3", cost = 3.5e5, department = 3,  multiplier = 3 },
        new TeamUpgrade { title = "Title Sponsor",        description = "All prize x3",     cost = 8e5,   department = -1, multiplier = 3 },
    };

    void Start() {
        panel.SetActive(false);
        Apply();
    }

    void Update() {
        if (badge != null)
            badge.SetActive(upgrades.Any(u => !u.purchased && CurrencyManager.Instance.money >= u.cost));
    }

    public void Open()  { panel.SetActive(true); Rebuild(); }
    public void Close() => panel.SetActive(false);

    void Rebuild() {
        foreach (Transform child in listParent) Destroy(child.gameObject);
        foreach (var u in upgrades.Where(x => !x.purchased).OrderBy(x => x.cost).Take(visibleCount))
            Instantiate(rowPrefab, listParent).Setup(u, this);
    }

    public void Buy(TeamUpgrade u) {
        if (u.purchased || !CurrencyManager.Instance.TrySpend(u.cost)) return;
        u.purchased = true;
        Apply();
        Rebuild();
    }

    public void Apply() {
        var depts = stats.departments;
        foreach (var d in depts) d.upgradeMultiplier = 1;
        foreach (var u in upgrades.Where(x => x.purchased))
            for (int i = 0; i < depts.Length; i++)
                if (u.department < 0 || u.department == i) depts[i].upgradeMultiplier *= u.multiplier;
        stats.Recalculate();
        foreach (var d in depts) d.Refresh();
    }

    public bool[] GetPurchased() => upgrades.Select(u => u.purchased).ToArray();

    public void SetPurchased(bool[] flags) {
        for (int i = 0; i < upgrades.Count && i < flags.Length; i++) upgrades[i].purchased = flags[i];
        Apply();
    }
}
