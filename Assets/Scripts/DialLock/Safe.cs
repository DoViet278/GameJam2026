using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Safe : MonoBehaviour
{
    [SerializeField] private GameObject dialLockUI;
    private bool isPlayerNear = false;

    private void OnCollisionStay2D(Collision2D collision)
    {
        isPlayerNear = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isPlayerNear = false;
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            dialLockUI.SetActive(true);
        }
        if (GameController.instance.safeOpended)
        {
            // Mo ket thanh cong
        }
    }

    private void OpenTheSafe() 
    {
        
    }
}
