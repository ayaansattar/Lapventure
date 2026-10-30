using UnityEngine;

[CreateAssetMenu(menuName = "Game/Department Config")]
public class DepartmentConfig : ScriptableObject
{
    public string displayName;
    public double baseCost = 10;
    public double costGrowth = 1.15;
    public float speedPerLevel = 0.05f;   // 0.05 = +5% speed per level
    public double incomePerLevel = 0;     // extra $ per lap per level
    public int[] milestones = { 10, 25, 50 };
    public float milestoneMultiplier = 2f;
}
