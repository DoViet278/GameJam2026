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
    [SerializeField] private GameObject tutorialKey;
    [SerializeField] private GameObject dialogSystem;
    [SerializeField] private GameObject rolePlay;
    [SerializeField] private GameObject tutorialQuest;
    private GameObject outroVideo;
    private bool isShowRolePlay = true;    
    private const string TutorialSeenKey = "Game.TutorialSeen"; 

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        outroVideo = GameObject.Find("Outro");
        
        if (GameController.instance.isShowDialog)
        {
            ShowDialogSystem();
            GameController.instance.isShowDialog = false;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            isShowRolePlay = !isShowRolePlay;
            rolePlay.SetActive(!isShowRolePlay);
        }
    }

    public void LoadHome()
    {
        outroVideo?.SetActive(false);
        GameController.instance.isGameOver = false;
        SceneManager.LoadScene(homeScene);
    }

    public void LoadMain()
    {
        outroVideo?.SetActive(false);
        AudioManager.Instance.musicSource.Play();
        GameController.instance.isGameOver = false;
        SceneManager.LoadScene(mainScene);
    }
    public void ShowTutorialKey()
    {
        tutorialKey.SetActive(true);
    }

    public void HideTutorialKey()
    {
        tutorialKey.SetActive(false);
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

    public void ShowDialogSystem()
    {
        dialogSystem.SetActive(true);
    }

    public void ShowRolePlay()
    {
        rolePlay.SetActive(true);
    }

    public void ShowTutorialQuest()
    {
        tutorialQuest.SetActive(true);
    }

    public void HideTutorialQuest()
    {
        tutorialQuest.SetActive(false);
    }
}
