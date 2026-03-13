using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.SetActive(false);
            UIConntroller.instance.ShowRolePlay();
            PlayerController.instance.EnableInput();
        }   
    }

    private void OnDisable()
    {
        AudioManager.Instance.musicSource.Play();
    }
}
