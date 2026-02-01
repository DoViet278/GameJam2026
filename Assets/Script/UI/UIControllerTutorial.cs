using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIControllerTutorial : MonoBehaviour
{
    [SerializeField] private Button btnPlay;

    public void ClickPlay()
    {
        SceneManager.LoadScene("MainScene");
    }
}
