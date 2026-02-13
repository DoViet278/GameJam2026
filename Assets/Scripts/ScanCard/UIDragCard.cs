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
    private Vector2 startPos;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPos = rectTransform.anchoredPosition;

    }

    private void OnEnable()
    {
        rectTransform.anchoredPosition = startPos;
        scanned = false;
        resultText.text = "Hãy quét thẻ để mở";
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
            resultText.text = "Vui lòng nhập mật khẩu!";
            GameController.instance.scannedCard = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
