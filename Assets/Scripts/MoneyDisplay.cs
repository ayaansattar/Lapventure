using UnityEngine;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{
    public TMP_Text label;
    void Start()
    {
        CurrencyManager.Instance.OnMoneyChanged += Refresh;
        Refresh(CurrencyManager.Instance.money);
    }
    void Refresh(double m) => label.text = "$" + NumberFormat.Format(m);
}
