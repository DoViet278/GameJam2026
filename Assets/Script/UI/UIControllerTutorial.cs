using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIControllerTutorial : MonoBehaviour
{
    [SerializeField] private Button btnPlay;
    [SerializeField] private TextMeshProUGUI txtDis;

    private void Start()
    {
        StartCoroutine(tutorialDiscription());
    }
    private IEnumerator tutorialDiscription()
    {
        txtDis.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        txtDis.gameObject.SetActive(false);
    }
    public void ClickPlay()
    {
        SceneManager.LoadScene("MainScene");
    }
}
