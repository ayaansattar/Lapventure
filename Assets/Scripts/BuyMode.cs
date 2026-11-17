using UnityEngine;

// Shared x1 / x10 / MAX buy setting for all departments.
public static class BuyMode {
    public static int Amount = 1;            // 1, 10, or 0 for MAX
    public static event System.Action Changed;
    public static void Cycle() {
        Amount = Amount == 1 ? 10 : Amount == 10 ? 0 : 1;
        Changed?.Invoke();
    }
    public static string Label => Amount == 0 ? "Buy MAX" : $"Buy x{Amount}";
}
