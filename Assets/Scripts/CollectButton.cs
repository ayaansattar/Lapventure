using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectButton : MonoBehaviour
{
    public LapPayout payout;
    public TMP_Text label;
    public Button button;
    public float pulseSpeed = 6f;      // how fast it pulses
    public float pulseAmount = 0.06f;  // how much it grows and shrinks
    public float popAmount = 0.2f;     // size of the pop when money arrives

    float pop;

    void Start()
    {
        payout.OnUncollectedChanged += Refresh;
        Refresh(0);
    }

    void Update()
    {
        pop = Mathf.MoveTowards(pop, 0f, Time.deltaTime * 1.5f);
        float pulse = payout.Uncollected > 0 ? Mathf.Sin(Time.time * pulseSpeed) * pulseAmount : 0f;
        float s = 1f + pulse + pop;
        transform.localScale = new Vector3(s, s, 1f);
    }

    void Refresh(double amount)
    {
        if (amount > 0) pop = popAmount;
        label.text = $"Collect ${NumberFormat.Format(amount)}";
        button.interactable = amount > 0;
    }
}