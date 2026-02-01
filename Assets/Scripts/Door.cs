using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;
    private BoxCollider2D boxCollider;
    private bool isOpen = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position,playerController.gameObject.transform.position) < 5f && playerController.isClickDoor)
        {
           if(!isOpen) StartCoroutine(OpenDoor());  
        }
    }

    private IEnumerator OpenDoor()
    {
        isOpen = true;
        animator.SetBool("open",true);
        boxCollider.enabled = false;
        yield return new WaitForSeconds(1f);
        isOpen = false;
    }

}
