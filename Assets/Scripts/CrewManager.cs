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
    public double mechanicBaseCost = 600, mechanicCostGrowth = 1.8;
    public int maxCrew = 6;
    double MechanicCost => mechanicBaseCost * System.Math.Pow(mechanicCostGrowth, pitStop.crewSize - 1);

    void Start()
    {
        if (payout.autoCollect) collectButton.SetActive(false);
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
        if (!pitStop.autoPit)
        {
            if (!CurrencyManager.Instance.TrySpend(pitCrewCost)) return;
            pitStop.autoPit = true;
        }
        else
        {
            if (pitStop.crewSize >= maxCrew || !CurrencyManager.Instance.TrySpend(MechanicCost)) return;
            pitStop.crewSize++;
        }
        Refresh();
    }

    void Refresh()
    {
        double money = CurrencyManager.Instance.money;
        engineerLabel.text = payout.autoCollect ? "Race Engineer\n(Hired)"
            : $"Race Engineer\n${NumberFormat.Format(engineerCost)}";
        engineerButton.interactable = !payout.autoCollect && money >= engineerCost;
        if (!pitStop.autoPit)
        {
            pitCrewLabel.text = $"Pit Crew\n${NumberFormat.Format(pitCrewCost)}";
            pitCrewButton.interactable = money >= pitCrewCost;
        }
        else if (pitStop.crewSize >= maxCrew)
        {
            pitCrewLabel.text = $"Pit Crew x{pitStop.crewSize}\n(Max)";
            pitCrewButton.interactable = false;
        }
        else
        {
            pitCrewLabel.text = $"Pit Crew x{pitStop.crewSize}\n+1 ${NumberFormat.Format(MechanicCost)}";
            pitCrewButton.interactable = money >= MechanicCost;
        }
    }
}