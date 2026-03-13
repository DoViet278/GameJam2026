using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Safe : MonoBehaviour
{
    private Animator animator;
    private bool isPlayerNear = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        

    }

    private void Start()
    {
        GameController.instance.safeOpended = false;
        GameController.instance.hasCard = false;
        animator.SetBool("open", false);  
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        isPlayerNear = true;
        UIConntroller.instance.ShowTutorialKey();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isPlayerNear = false;
        UIConntroller.instance.HideTutorialKey();
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !GameController.instance.safeOpended)
        {
            UIConntroller.instance.ShowDialLockUI();    
        }
        if (GameController.instance.safeOpended)
        {
            animator.SetBool("open", true);
        }
        if (GameController.instance.hasCard)
        {
            animator.SetBool("get", true);
        }
    }
}
