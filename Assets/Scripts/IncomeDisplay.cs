using UnityEngine;
using TMPro;

// Shows money earned per lap, e.g. "+$39/lap".
public class IncomeDisplay : MonoBehaviour {
    public LapPayout payout;
    public TMP_Text label;

    double shown = -1;

    void Update() {
        if (payout.moneyPerLap == shown) return;
        shown = payout.moneyPerLap;
        label.text = $"+${NumberFormat.Format(shown)}/lap";
    }
}
