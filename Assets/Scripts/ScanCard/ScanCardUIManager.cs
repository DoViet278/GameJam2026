using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScanCardUIManager : MonoBehaviour
{
    [SerializeField] private GameObject imgCard;
    [SerializeField] private GameObject inputPassword;
    [SerializeField] private Button btnClose;
    [Header("UI")]
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button checkBtn;
    private bool checkPassword;
    private string correctPassword = "GameJam2026";

    private void Start()
    {
        checkBtn.onClick.AddListener(CheckPassword);
        btnClose.onClick.AddListener(CloseScanCardUI);
        resultText.text = "";
    }

    private void OnEnable()
    {
        inputPassword.SetActive(false);
        if (GameController.instance.hasCard)
        {
            imgCard.SetActive(true);
        }
        else
        {
            imgCard.SetActive(false);
        }
    }

    private void Update()
    {
        if (GameController.instance.scannedCard)
        {
            inputPassword.SetActive(true);
        }
    }

    public void CloseScanCardUI()
    {
        UIConntroller.instance.HideScanCardUI();
    }
    public void CheckPassword()
    {
        string userInput = passwordInput.text;

        if (userInput == correctPassword)
        {
            resultText.text = "Mật khẩu đúng!";
            resultText.color = Color.green;
            GameController.instance.acceptedPassword = true;
            QuestManager.Instance.CompleteCurrentQuest();
            CloseScanCardUI();
        }
        else
        {
            resultText.text = "Sai mật khẩu!";
            resultText.color = Color.red;
        }
    }

    private void OnDisable()
    {
        GameController.instance.scannedCard = false;    
    }
}
