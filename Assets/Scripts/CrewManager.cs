using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrewManager : MonoBehaviour
{
    public LapPayout payout;
    public PitStopManager pitStop;
    public GameObject collectButton;
    public Button engineerButton, pitCrewButton;
    public TMP_Text engineerLabel, pitCrewLabel;
    public double engineerCost = 300, pitCrewCost = 1500;

    void Start()
    {
        CurrencyManager.Instance.OnMoneyChanged += _ => Refresh();
        Refresh();
    }

    public void HireEngineer()
    {
        if (payout.autoCollect || !CurrencyManager.Instance.TrySpend(engineerCost)) return;
        payout.Collect();
        payout.autoCollect = true;
        collectButton.SetActive(false);
        Refresh();
    }

    public void HirePitCrew()
    {
        if (pitStop.autoPit || !CurrencyManager.Instance.TrySpend(pitCrewCost)) return;
        pitStop.autoPit = true;
        Refresh();
    }

    void Refresh()
    {
        double money = CurrencyManager.Instance.money;
        engineerLabel.text = payout.autoCollect ? "Race Engineer\n(Hired)"
            : $"Race Engineer\n${NumberFormat.Format(engineerCost)}";
        engineerButton.interactable = !payout.autoCollect && money >= engineerCost;
        pitCrewLabel.text = pitStop.autoPit ? "Pit Crew\n(Hired)"
            : $"Pit Crew\n${NumberFormat.Format(pitCrewCost)}";
        pitCrewButton.interactable = !pitStop.autoPit && money >= pitCrewCost;
    }
}