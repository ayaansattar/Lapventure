using UnityEngine;

[CreateAssetMenu(menuName = "Game/Department Config")]
public class DepartmentConfig : ScriptableObject {
    public string displayName;
    public double baseCost = 1;              // cost of the first level
    public double costGrowth = 1.075;        // each level costs 7.5% more
    public double incomePerLevel = 1;        // $ per lap added by each level
    public int maxLevel = 150;
    public int[] stars = { 25, 50, 100, 150 };   // milestone levels
    public double starMultiplier = 2;        // income multiplier per star reached
}
