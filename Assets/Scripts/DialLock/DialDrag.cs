using UnityEngine;
using UnityEngine.EventSystems;

public class DialDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public SafeDialController controller;

    public void OnPointerDown(PointerEventData eventData)
    {
        controller.StartDrag();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        controller.EndDrag();
    }
}
