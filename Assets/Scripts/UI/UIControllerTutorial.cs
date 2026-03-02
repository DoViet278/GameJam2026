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
        txtDis.text = "Đến tủ quần áo để thay đồ";
        yield return new WaitForSeconds(4f);
        txtDis.text = "Tìm kiếm vật phẩm ở kệ";
    }
    public void ClickPlay()
    {
        SceneManager.LoadScene("MainScene");
    }
}
