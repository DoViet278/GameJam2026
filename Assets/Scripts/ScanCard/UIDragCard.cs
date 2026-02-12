using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragCard : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    public RectTransform scanArea;
    public TextMeshProUGUI messageText;
    private bool scanned = false;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
             eventData.delta / canvas.scaleFactor;

        if (!scanned && IsOverlapping())
        {
            scanned = true;
            messageText.text = "Đã quét thành công!";
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    bool IsOverlapping()
    {
        return RectTransformUtility
            .RectangleContainsScreenPoint(
                scanArea,
                rectTransform.position,
                canvas.worldCamera
            );
    }
}
