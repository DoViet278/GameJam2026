using UnityEngine;
using UnityEngine.SceneManagement;

public class UIConntroller : MonoBehaviour
{
    public static UIConntroller instance;

    [Header("Scene Names")]
    [SerializeField] private string homeScene = "Home";
    [SerializeField] private string tutorialScene = "Tutorial";
    [SerializeField] private string mainScene = "MainScene";
    [SerializeField] private GameObject uiDialLock;
    [SerializeField] private GameObject uiScanCard;
    private GameObject outroVideo;

    private const string TutorialSeenKey = "Game.TutorialSeen";
    private bool isFirstTimePlaying;    

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        outroVideo = GameObject.Find("Outro");
    }

    public void StartGame()
    {
        if (HasSeenTutorial())
            LoadMain();
        else
            LoadTutorial();
    }

    public void LoadHome()
    {  
        outroVideo?.SetActive(false);
        GameController.instance.isGameOver = false;
        SceneManager.LoadScene(homeScene);
    }

    public void LoadTutorial()
    {
        SetTutorialSeen(true);
        SceneManager.LoadScene(tutorialScene);
    }

    public void LoadMain()
    {
        outroVideo?.SetActive(false);
        GameController.instance.isGameOver = false;
        SceneManager.LoadScene(mainScene);
    }

    public void CloseToHome()
    {
        LoadHome();
    }

    public void NextFromTutorial()
    {
        SetTutorialSeen(true);
        LoadMain();
    }

    public void ResetTutorialSeen()
    {
        SetTutorialSeen(false);
    }

    private bool HasSeenTutorial()
    {
        return PlayerPrefs.GetInt(TutorialSeenKey, 0) == 1;
    }

    private void SetTutorialSeen(bool seen)
    {
        PlayerPrefs.SetInt(TutorialSeenKey, seen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ShowScanCardUI()
    {
        Time.timeScale = 0f;
        uiScanCard.SetActive(true);
    }

    public void HideScanCardUI()
    {
        Time.timeScale = 1f;
        uiScanCard.SetActive(false);
    }   

    public void ShowDialLockUI()
    {
        PlayerController.instance.isOpenSafe = true;
        uiDialLock.SetActive(true);
    }

    public void HideDialLockUI()
    {
        PlayerController.instance.isOpenSafe = false;
        uiDialLock.SetActive(false);
    }
}
