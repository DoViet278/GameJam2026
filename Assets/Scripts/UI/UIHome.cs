using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHome : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string homeScene = "Home";
    [SerializeField] private string tutorialScene = "Tutorial";
    [SerializeField] private string mainScene = "MainScene";
    private const string TutorialSeenKey = "Game.TutorialSeen";
    public void StartGame()
    {
        if (HasSeenTutorial())
            LoadMain();
        else
            LoadTutorial();
    }

    public void LoadHome()
    {
        SceneManager.LoadScene(homeScene);
    }

    public void LoadTutorial()
    {
        SetTutorialSeen(true);
        SceneManager.LoadScene(tutorialScene);
    }

    public void LoadMain()
    {
        AudioManager.Instance.musicSource.Play();
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
}
