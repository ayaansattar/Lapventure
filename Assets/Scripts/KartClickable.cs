using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(KartMover))]
public class KartClickable : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData e) =>
        KartInfoPopup.Instance.Show(GetComponent<KartMover>());
}