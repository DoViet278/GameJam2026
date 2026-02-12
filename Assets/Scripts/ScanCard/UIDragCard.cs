using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragCard : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform scanArea;
    public TextMeshProUGUI resultText;

    [Header("Scan Settings")]
    public float centerThreshold = 10f;  

    private RectTransform rectTransform;
    private Canvas canvas;
    private bool scanned = false;
    private Vector2 lastPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (scanned) return;

        lastPos = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (scanned) return;

        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;

        // Kiểm tra đang kéo từ phải qua trái
        if (rectTransform.anchoredPosition.x < lastPos.x)
        {
            CheckScan();
        }

        lastPos = rectTransform.anchoredPosition;
    }

    void CheckScan()
    {
        Vector2 cardCenter = rectTransform.position;
        Vector2 scanCenter = scanArea.position;

        float distance = Vector2.Distance(cardCenter, scanCenter);

        if (distance <= centerThreshold)
        {
            scanned = true;
            resultText.text = "Đã quét thành công!";
            Debug.Log("Scan success");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
