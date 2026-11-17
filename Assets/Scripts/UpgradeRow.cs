using UnityEngine;
using UnityEngine.UI;
using TMPro;

// One row in the Upgrades popup.
public class UpgradeRow : MonoBehaviour {
    public Image icon;
    public TMP_Text title;
    public TMP_Text description;
    public TMP_Text costLabel;
    public Button button;

    TeamUpgrade upgrade;

    public void Setup(TeamUpgrade u, TeamUpgradesManager manager) {
        upgrade = u;
        title.text = u.title;
        description.text = u.description;
        costLabel.text = "$" + NumberFormat.Format(u.cost);
        if (icon != null) {
            icon.sprite = u.icon;
            icon.enabled = u.icon != null;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => manager.Buy(u));
    }

    void Update() {
        if (upgrade == null) return;
        bool affordable = CurrencyManager.Instance.money >= upgrade.cost;
        button.interactable = affordable;
        costLabel.color = affordable ? Color.black : new Color(0.85f, 0.2f, 0.2f);
    }
}
