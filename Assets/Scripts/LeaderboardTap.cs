using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class LeaderboardTap : MonoBehaviour, IPointerClickHandler
{
    public RaceManager race;
    public TMP_Text text;

    public void OnPointerClick(PointerEventData e)
    {
        int line = TMP_TextUtilities.FindNearestLine(text, e.position, null);
        Debug.Log($"Leaderboard tapped, line {line}");
        if (line < 0 || race.Order == null || line >= race.Order.Length) return;
        KartInfoPopup.Instance.Show(race.Order[line]);
    }
}