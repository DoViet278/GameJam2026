using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SafeDialController : MonoBehaviour
{
    [Header("Dial")]
    public RectTransform dialRect;
    public TextMeshProUGUI numberText;
    public TextMeshProUGUI resultText;

    [Header("Password")]
    public List<int> correctCode = new List<int> { 30, 10, 70 };
    private List<int> inputCode = new List<int>();

    private bool isDragging = false;
    private float currentAngle;
    private float dragOffset;
    

    void Update()
    {
        if (isDragging)
        {
            RotateDial();
        }
    }

    void RotateDial()
    {
        Vector2 center = RectTransformUtility.WorldToScreenPoint(null, dialRect.position);
        Vector2 dir = (Vector2)Input.mousePosition - center;

        float mouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float targetAngle = mouseAngle - dragOffset;

        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * 15f);

        dialRect.localRotation = Quaternion.Euler(0, 0, currentAngle);

        int number = Mathf.RoundToInt((currentAngle + 360f) % 360f / 360f * 100f);
        numberText.text = number.ToString("00");
    }

    public void StartDrag()
    {
        isDragging = true;

        Vector2 center = RectTransformUtility.WorldToScreenPoint(null, dialRect.position);
        Vector2 dir = (Vector2)Input.mousePosition - center;

        float mouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        dragOffset = mouseAngle - currentAngle;
    }

    public void EndDrag()
    {
        isDragging = false;

        int number = Mathf.RoundToInt((currentAngle + 360f) % 360f / 360f * 100f);
        inputCode.Add(number);

        CheckProgress();
    }

    void CheckProgress()
    {
        int index = inputCode.Count - 1;

        if (inputCode[index] != correctCode[index])
        {
            resultText.text = "Sai mật mã. Thử lại!";
            resultText.color = Color.red;
            inputCode.Clear();
            return;
        }

        if (inputCode.Count == correctCode.Count)
        {
            resultText.text = "Mở két thành công";
            resultText.color = Color.green;
            inputCode.Clear();
            GameController.instance.safeOpended = true;
            UIConntroller.instance.HideDialLockUI();
        }
        else
        {
            resultText.text = "Đúng, tiếp tục...";
            resultText.color = Color.white;
        }
    }

    public void OnClickClose()
    {
        UIConntroller.instance.HideDialLockUI();
        inputCode.Clear();
    }
}
