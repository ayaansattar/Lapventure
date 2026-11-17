using UnityEngine;
using TMPro;

public class BuyModeToggle : MonoBehaviour {
    public TMP_Text label;
    void Start() => label.text = BuyMode.Label;
    public void Cycle() {                    // hook the button's On Click to this
        BuyMode.Cycle();
        label.text = BuyMode.Label;
    }
}
