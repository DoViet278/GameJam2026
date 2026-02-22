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
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !GameController.instance.safeOpended)
        {
            UIConntroller.instance.ShowDialLockUI();    
        }
        if (GameController.instance.safeOpended)
        {
            animator.SetBool("open", true);
            QuestManager.Instance.CompleteCurrentQuest();
        }
        if (GameController.instance.hasCard)
        {
            animator.SetBool("get", true);
        }
    }
}
